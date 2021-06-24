using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// NOT IN USE

public class Mover : MonoBehaviour
{
    public GameObject gfx, attackObj;
    public float moveSpeed, attackCooldown, offset;

    float lastAttack;
    Rigidbody2D thisRigidbody;
    SpriteRenderer thisRenderer;

    void Start()
    {
        thisRigidbody = GetComponent<Rigidbody2D>();
        thisRenderer = GetComponentInChildren<SpriteRenderer>();
        UpdateDepth();
        lastAttack = -attackCooldown;
    }

    void Update()
    {
        Vector2 movePos = new Vector2(Input.GetAxis("Horizontal"), 
                                      Input.GetAxis("Vertical"));
        thisRigidbody.MovePosition(new Vector2(transform.position.x,transform.position.y) + movePos * moveSpeed * Time.deltaTime);

        if(Input.GetButtonDown("Jump")){
            Attack();
        }

        UpdateDepth();

    }

    void UpdateDepth(){
        if (Mathf.Abs(gfx.transform.position.z - (transform.position.y + offset) / 10) > Mathf.Epsilon)
        {
            gfx.transform.localPosition = new Vector3(0, 0, (transform.position.y + offset) / 10);
        }
    }

    private void Attack(){
        if(lastAttack + attackCooldown < Time.time){
            lastAttack = Time.time;
            StartCoroutine(AttackCo());
        }
    }

    IEnumerator AttackCo(){
        attackObj.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        if(attackObj.activeInHierarchy){
            attackObj.SetActive(false);
        }
        yield return null;
    }

    //DEBUG//

    private void OnDrawGizmos()
    {
        if (thisRenderer != null)
        {
            Vector3 offsetVector = new Vector3(0, offset, 0);
            Vector3 extendsVector = new Vector3(thisRenderer.bounds.extents.x, 0, 0);
            Gizmos.color = Color.magenta;

            Gizmos.DrawLine(thisRenderer.bounds.center - extendsVector + offsetVector, thisRenderer.bounds.center + extendsVector + offsetVector);
        }
    }
}
