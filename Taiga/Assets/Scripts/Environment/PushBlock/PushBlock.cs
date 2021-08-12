using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction{
    up,
    down,
    left,
    right
}
public class PushBlock : MonoBehaviour
{

    public MoveTrigger up,down,left,right;
    public int pushAmount;
    int _pushAmount;
    int moveTicker = 0;
    [SerializeField]
    public float pushLength, pushDuration, pushDelay;
    float pushTimer;
    private bool pushable = true, push, moving;
    private Rigidbody2D thisRB;
    Vector2 startPos, endPos;
    WorldObject worldObject;

    private void OnEnable()
    {
        _pushAmount = pushAmount;
    }

    private void Start()
    {
        worldObject = GetComponentInChildren<WorldObject>();
        push = false;
        thisRB = GetComponent<Rigidbody2D>();  
    }

    private void Update()
    {
        // Only try to push if block has push amount left, or is set to negative value eg. infinate pushable
        if (_pushAmount == 0)
        {
            push = false;
            return;
        }
            // run push-move if push is true
            if (push)
            {
                moving = true;
                worldObject.SetDepth(transform.position);

                if(moveTicker == 0){
                    pushTimer = Time.time;
                    moveTicker = 1;
                }

                if(Time.time >= pushTimer + moveTicker * pushDuration/16){
                    thisRB.MovePosition(new Vector2(startPos.x + moveTicker * (endPos.x - startPos.x) / 16, startPos.y + moveTicker * (endPos.y - startPos.y) / 16));
                    moveTicker++;

                }if(Time.time >=pushTimer + pushDuration){
                    thisRB.MovePosition(endPos);
                    worldObject.SetDepth(endPos);
                    push = false;
                    moving = false;
                    moveTicker = 0;
                    if (_pushAmount > 0){
                        _pushAmount--;
                    }
                }
            }

    }

    public void SetPushable(bool state)
    {
        pushable = state;
    }
    private void OnTriggerStay2D(Collider2D other)
    {

        if (_pushAmount != 0 && other.isTrigger && other.GetComponent<Player>() && pushable && !moving)
        {
            MoveTrigger activeTrigger;
            Direction pushFrom;
            if (up.playerOccupied)
            {
                activeTrigger = up;
                pushFrom = Direction.up;
            }
            else if (down.playerOccupied)
            {
                activeTrigger = down;
                pushFrom = Direction.down;
            }
            else if (left.playerOccupied)
            {
                activeTrigger = left;
                pushFrom = Direction.left;
            }
            else if (right.playerOccupied)
            {
                activeTrigger = right;
                pushFrom = Direction.right;
            }
            else { return; }

            Player player = other.GetComponent<Player>();
            StartCoroutine(TryPush(player, activeTrigger, pushFrom));
        }
    }

    IEnumerator TryPush(Player player,MoveTrigger pushTrigger, Direction direction){
        bool currentPushable = pushable;
        Vector2 localStartPos, localEndPos;
        localStartPos = transform.position;

        //Endposition is decided by the direction of the MoveTrigger being activated
        localEndPos = localStartPos - (pushTrigger.direction * pushLength);
        //If target grid is occupied, dont try to push
        switch(direction){
            case Direction.up:
                if (down.isOccupied)    { yield break; }
                break;
            case Direction.down:
                if (up.isOccupied)      { yield break; }
                break;
            case Direction.left:
                if (right.isOccupied)   { yield break; }
                break;
            case Direction.right:
                if (left.isOccupied)    { yield break; }
                break;   
        }
        //only allow one instance of this coroutine per cycle
        SetPushable(false);

        yield return new WaitForSeconds(pushDelay);
        Vector2 vectorSum = pushTrigger.direction + player.lookDirection;
        //Only try to push if player is moving, and is moving in the direction of the push, and the push trigger is still occupied
        if (vectorSum.magnitude < Mathf.Epsilon && (player.GetState() == State.moving) && pushTrigger.playerOccupied)
        {
            startPos = localStartPos;
            endPos = localEndPos;
            push = true;
        }
       
        yield return new WaitForSeconds(pushDuration);
        SetPushable(currentPushable);
    }
}
