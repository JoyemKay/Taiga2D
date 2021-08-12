using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Character : MonoBehaviour
{

    State stateBeforePause;
    public bool canAttack;
    public float moveSpeed, attackSpeed, offset;
    public GameObject gfx;
    public Transform target;
    public Vector2 lookDirection;
    protected bool onAttackCooldown, canMove, hasMoved;
    protected int currentFloor;
    protected float lastAttackTime;
    protected Rigidbody2D thisRigidbody;
    protected SpriteRenderer gfxRenderer;
    protected Animator gfxAnimator;

    [SerializeField]
    protected State state;
    protected State previousState;
    GameObject attackObject;


    #region Inheritance functions with funcitonality in childrens
    protected virtual void Start()
    {
        thisRigidbody = GetComponent<Rigidbody2D>();
        gfxRenderer = gfx.GetComponent<SpriteRenderer>();
        gfxAnimator = GetComponent<Animator>();
        lastAttackTime = Time.time - attackSpeed;
        UpdateDepth();
        if (GetComponentInChildren<Damage>())
        {
            attackObject = GetComponentInChildren<Damage>().gameObject;
            attackObject.SetActive(false);
        }
    }

    protected virtual void Update()
    {
        if (GameController.Instance.IsPaused && state != State.paused) { Pause(); }
        else
        if (!GameController.Instance.IsPaused && state == State.paused) { Resume(); }

        if (state != State.paused && state != State.staggered && state != State.attacking)
        {
            SetState(State.idle);
            Move();
            Attack();
            Interact();
        }
        UpdateDepth();
        previousState = state;
        hasMoved = false;
    }

    protected virtual void Move()
    {
        //  Should be overwritten
        //  Some condition determining if canMove = false;
        canMove = true;
    }


    protected virtual void Attack()
    {
        //  Should be overwritten, only controls that attackspeed is passed by before allowing attack
        onAttackCooldown = !(Time.time > lastAttackTime + attackSpeed);
    }

    protected virtual void Interact(){
        
    }

    protected virtual void Pause()
    {
        stateBeforePause = state;
        state = State.paused;
    }

    protected virtual void Resume()
    {
        state = stateBeforePause;
    }

    protected virtual void Die(){
        
    }

    #endregion


    #region animation

    //flips gfx horizontally if flip and current scale doesnt match
    public void Flip(float flip)
    {
        if (Mathf.Sign(gfx.transform.localScale.x) != flip)
        {
            gfx.transform.localScale = new Vector3(flip, 1);
        }
    }

    public void SetAnimatorValue(string boolName, bool value)
    {
        gfxAnimator.SetBool(boolName, value);
    }

    public void SetAnimatorValue(string boolName, float value)
    {
        gfxAnimator.SetFloat(boolName, value);
    }

    public void SetAnimatorValue(string name)
    {
        gfxAnimator.SetTrigger(name);
    }

    #endregion

    //Moves the character to position, only callable once per frame.
    public void MoveTo(Vector2 position)
    {
        if (!hasMoved) { thisRigidbody.MovePosition(position); }
        hasMoved = true;
    }

    public void SetPosition(Vector2 position)
    {
        transform.position = position;
    }

    public void SetFloor(int floor)
    {

        currentFloor = floor;
        gfxRenderer.sortingOrder = floor * 10 + 5;
        GameController.Instance.SetColliderLayer(this.gameObject, floor);
    }

    //  Enables the attack object, initiating an attack. Should be called from animator
    public void ToggleAttack()
    {
        if (attackObject)
        {
            attackObject.SetActive(true);
        }
    }

    // Disables the attack object at end of animation, called from animator
    public void DisableAttack()
    {
        if (attackObject)
        {
            attackObject.SetActive(false);
        }
    }
    //  Sets the correct z-pos based on the current y-pos (for pseudo-3D depth)
    void UpdateDepth()
    {
        if (Mathf.Abs(gfx.transform.position.z - (transform.position.y + offset) / 100)
            >
            Mathf.Epsilon) { gfx.transform.localPosition = new Vector3(0, gfx.transform.localPosition.y, (transform.position.y + offset) / 100); }
    }

    public void SetState(State newState){
        state = newState;
    }

    public State GetState()
    {
        return state;
    }

    public int GetFloor()
    {
        return currentFloor;
    }


    public void TryStagger(float duration){
        if(state != State.staggered){
            StartCoroutine(StaggerCharacter(this,duration));
        }
    }

    IEnumerator StaggerCharacter(Character character, float duration){
        character.SetState(State.staggered);
        float start = Time.time;
        while(start + duration > Time.time){
            yield return null;
        }
        if (character)
        {
            character.SetState(State.staggered);
        }
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

    public void ResumePath()
    {
        if (AstarPath != null)
        {

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
                if ((transform.position - currentWaypoint).sqrMagnitude < tolerance)

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

    public void StopAstarMove()
    {
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
