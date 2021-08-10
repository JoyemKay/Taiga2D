using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTransition : MonoBehaviour
{
    BoxCollider2D transitionCollider;
    float transitionDelay = 1,timer = 0;
    public bool isLevelTransition = false;
    public string levelToLoad;
    //TODO: add appropriate value for name or varibable to next level.
    public float extraDistance = 0;
    public TransitionDirection direction;
    Player player;

    // Start is called before the first frame update
    void Start()
    {
        transitionCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(timer>transitionDelay){
            Debug.Log("Transition requirements met, transitioning player...");
            //Should be called via GameController
            GameController.Instance.TransitionPlayer(this, isLevelTransition);
            timer = 0;
        }
    }

    //Moves the player to the other side of the transition object, with extraDistance from center of object
    public void Transition(){
        Vector2 exitPosition = Vector2.zero;
        float directionSign;

        if(direction == TransitionDirection.horizontal){
            directionSign = Mathf.Sign(transform.position.x - player.transform.position.x);
            exitPosition = new Vector2(transform.position.x + directionSign * (transitionCollider.size.x/2 + extraDistance),player.transform.position.y);
        }
        else if(direction == TransitionDirection.vertical){
            directionSign = Mathf.Sign(transform.position.y - player.transform.position.y);
            exitPosition = new Vector2(player.transform.position.x, transform.position.y + directionSign * (transitionCollider.size.y/2 + extraDistance) - player.offset);
        }
        //Not MoveTo because one-way transitions should be possible
        player.SetPosition(exitPosition);
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.CompareTag("Player") && !other.isTrigger){
            if(!player){
                Debug.Log("Player has entered transition: " + name);
                player = other.GetComponent<Player>();
                transitionDelay = 1 / (player.moveSpeed + 1);
            }


            Vector2 playerPos = player.transform.position;
            if(direction == TransitionDirection.horizontal){
                float relativePosition = Mathf.Sign(transform.position.x - player.transform.position.x);

                if (Mathf.Abs(player.lookDirection.x - relativePosition) < Mathf.Epsilon)
                {
                    //Continue
                }
                else { return; }

            }
            else if(direction == TransitionDirection.vertical){
                float relativePosition = Mathf.Sign(transform.position.y - player.transform.position.y);

                if (Mathf.Abs(player.lookDirection.y - relativePosition) < Mathf.Epsilon)
                {
                    //Continue
                }
                else { return; }
            }
            timer += Time.deltaTime;

        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            timer = 0;
            transitionDelay = 1;
            player = null;
        }
    }
}
