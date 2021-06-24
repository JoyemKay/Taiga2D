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


    //  Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        interactOffset = interactCollider.offset;
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
        if(canMove){
            
            //Disables gliding after pause state
            if(previousState == State.paused){
                Input.ResetInputAxes();
                return;
            }
            Vector2 movePos = new Vector2(Input.GetAxis("Horizontal"),
                              Input.GetAxis("Vertical"));


            if(movePos.sqrMagnitude > Mathf.Epsilon){
                state = State.moving;
                lookDirection = movePos.normalized;

                MoveTo(new Vector2(transform.position.x, transform.position.y) + movePos * moveSpeed * Time.deltaTime);
            }
        }

    }

    protected override void Attack()
    {
        base.Attack();
        //  Only attack if able to
        if(Input.GetButtonDown("Attack") && canAttack){
            //  TODO: Start attack animation
            //  For debugging purposes only, enabling attack hitbox should be done in animaton
            ToggleAttack();
            lastAttackTime = Time.time;
        }
    }
}
