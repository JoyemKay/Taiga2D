using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Character : MonoBehaviour
{
    
    State stateBeforePause;
    protected State previousState;
    public float moveSpeed, attackSpeed, offset;
    public GameObject gfx;
    public Transform target;
    public Vector2 lookDirection;
    protected bool canAttack,canMove,hasMoved;
    protected float lastAttackTime;
    protected  Rigidbody2D thisRigidbody;
    protected SpriteRenderer gfxRenderer;
    [SerializeField]
    protected State state;
    GameObject attackObject;

#region Inheritance functions with funcitonality in childrens
    protected virtual void Start()
    {
        thisRigidbody = GetComponent<Rigidbody2D>();
        gfxRenderer = gfx.GetComponent<SpriteRenderer>();
        lastAttackTime = Time.time - attackSpeed;
        UpdateDepth();
        if (GetComponentInChildren<Damage>()){
            attackObject = GetComponentInChildren<Damage>().gameObject;
            attackObject.SetActive(false);
        }
    }

    protected virtual void Update()
    {
        if (GameController.Instance.IsPaused && state != State.paused) { Pause(); }
        else
        if (!GameController.Instance.IsPaused && state == State.paused) { Resume(); }

        if (state != State.paused && state != State.staggered)
        {
            state = State.idle; 
            Move();
            Attack();
        }
        UpdateDepth();
        previousState = state;
        hasMoved = false;
    }

    protected virtual void Move(){
        //  Should be overwritten
        //  Some condition determining if canWalk = false;
        canMove = true;
    }

    //Moves the character to position, only callable once per frame.
    public void MoveTo(Vector2 position){
        if (!hasMoved) { thisRigidbody.MovePosition(position); }
        hasMoved = true;
    }

    public void SetPosition(Vector2 position){
        transform.position = position;
    }

    protected virtual void Attack(){
        //  Should be overwritten, only controls that attackspeed is passed by before allowing attack
        canAttack = Time.time > lastAttackTime + attackSpeed;
    }

    protected virtual void Pause(){
        stateBeforePause = state;
        state = State.paused;
    }

    protected virtual void Resume(){
        state = stateBeforePause;
    }

#endregion

    //  Enables the attack object, initiating an attack. Should be called from animator
    public void ToggleAttack(){
        if (attackObject)
        {
            attackObject.SetActive(true);
        }
    }

    //  Sets the correct z-pos based on the current y-pos (for pseudo-3D depth)
    void UpdateDepth()
    {
        if (Mathf.Abs(gfx.transform.position.z - (transform.position.y + offset) / 100) 
            > 
            Mathf.Epsilon) { gfx.transform.localPosition = new Vector3(0, gfx.transform.localPosition.y, (transform.position.y + offset) / 100); }
    }

    public State GetState(){
        return state;
    }

#region Astar-related scripts
    Vector3[] AstarPath;

    protected void AstarMove()
    {
        AstarPathRequestManager.RequestPath(transform.position, 
                                            target.position, 
                                            OnPathFound);
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            AstarPath = newPath;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    public void ResumePath(){
        if(AstarPath != null){
            
        }
    }

    int targetIndex = 0;
    IEnumerator FollowPath()
    {
        Vector3 offsetVector = new Vector3(0, -offset, 0);
        Vector3 currentWaypoint = AstarPath[0] + offsetVector;

        float tolerance = 0.008f;
        while (true)
        {
            //  Only move towards next position if not paused or staggered
            if (state != State.paused || state != State.staggered)
            {
                     if ((transform.position  - currentWaypoint).sqrMagnitude < tolerance)

                {
                    targetIndex++;
                    if (targetIndex >= AstarPath.Length) 
                    {
                        AstarPath = null;
                        yield break; 
                    }
                    currentWaypoint = AstarPath[targetIndex] + offsetVector;
                }

                Vector2 pos = new Vector2(transform.position.x, transform.position.y);
                Vector2 movePos = new Vector2(currentWaypoint.x - pos.x,
                                              currentWaypoint.y - pos.y).normalized;
                MoveTo(pos + movePos * moveSpeed * Time.deltaTime);
                state = State.moving;
            }
            yield return null;
        }
    }

    public void StopAstarMove(){
        StopCoroutine("FollowPath");
        AstarPath = null;
        state = State.idle;
    }

    public void OnDrawGizmos()
    {
        if (AstarPath != null)
        {
            for (int i = targetIndex; i < AstarPath.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(AstarPath[i], Vector3.one * 0.5f);

                if (i == targetIndex) { Gizmos.DrawLine(transform.position, AstarPath[i]); }
                else { Gizmos.DrawLine(AstarPath[i - 1], AstarPath[i]); }
            }
        }
    }
#endregion

}
