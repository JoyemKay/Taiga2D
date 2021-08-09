using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : Character
{
    [SerializeField]
    public Collider2D interactCollider;
    Vector2 interactOffset;
    public float interactDist;
    public bool canInteract = true;
    bool hasStarted = false;

    public void Initiate(int floor)
    {
        Start();
        SetFloor(floor);

    }

    //  Start is called before the first frame update
    protected override void Start()
    {
        if (!hasStarted)
        {
            base.Start();
            interactOffset = interactCollider.offset;
            hasStarted = true;
        }
    }

    //  Update is called once per frame
    protected override void Update()
    {
        base.Update();
        interactCollider.offset = interactOffset + lookDirection * (interactDist);
    }

    protected override void Move()
    {
        base.Move();
        if (canMove)
        {

            //Disables gliding after pause state
            if (previousState == State.paused)
            {
                Input.ResetInputAxes();
                return;
            }
            Vector2 movePos = new Vector2(
                                Input.GetAxis("Horizontal"),
                                Input.GetAxis("Vertical"));
            

            if (movePos.sqrMagnitude > Mathf.Epsilon)
            {
                SetState(State.moving);
                SetAnimatorValue("isMoving", true);
                lookDirection = movePos.normalized;
                SetAnimatorValue("lookX", lookDirection.x);
                SetAnimatorValue("lookY", lookDirection.y);
                Flip(Mathf.Sign(lookDirection.x));

                MoveTo(new Vector2(transform.position.x, transform.position.y) + movePos * moveSpeed * Time.deltaTime);
            }else{
                SetAnimatorValue("isMoving", false);
            }
        }

    }

    protected override void Attack()
    {
        base.Attack();
        //  Only attack if able to
        if (Input.GetButtonDown("Attack") && canAttack)
        {
            SetState(State.attacking);
            SetAnimatorValue("attack");
            lastAttackTime = Time.time;
        }
    }
}
