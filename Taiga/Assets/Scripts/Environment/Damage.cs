using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//  Generic class for trigger colliders that invokes a damage reaction on target
//  Layer on damage object dictates what other objects can be interacted with
public class Damage : MonoBehaviour
{
    float activeTime = 0.1f;
    bool hasHit;
    [SerializeField]
    float damage;
    public bool oneTime = true, triggerOnObject = true;


    private void OnTriggerEnter2D(Collider2D other)
    {

        // Only deal damage to first valid target hit
        if (!hasHit)
        {
            Debug.Log("Damage object on " + transform.parent.name + " has interacted with "+other.name);
            /* DEPRECATED
            //  If colliding collider is a destructable, 
            //  start objects destroy animation, then disable damage object
            Destructable destructable = other.gameObject.GetComponent<Destructable>();
            if (destructable)
            {
                destructable.StartDestroyAnimator();
                hasHit = true;
                gameObject.SetActive(false);
                return;
            }
            */
            //TODO: Check if target is damagable, if so, deal damage

            //If damage object can interact with destructable Interactables, do so
            if (triggerOnObject)
            {
                InteractOnDamage targetObject = other.GetComponent<InteractOnDamage>();
                if (targetObject)
                {
                    Debug.Log("Hit destructable: " + other.name);
                    targetObject.Interact();
                    if (oneTime) { hasHit = true; }
                    return;
                }
            }

            //Damage target character
            Character targetCharacter = other.GetComponent<Character>();
            if(targetCharacter){
                Debug.Log("Hit character: " + other.name);
                DoDamage(targetCharacter);
                if (oneTime) { hasHit = true; }
                return;
            }
        }
    }

    void DoDamage(Character character){
        //Do something
    }

    public void EnableAttack()
    {
        gameObject.SetActive(true);
    }

    public void DisableAttack()
    {
        gameObject.SetActive(false);
    }

    //  Resets the hasHit variable when object is enabled   
    private void OnEnable()
    {
        hasHit = false;
        //  DEBBUGING ONLY, state should be set in animator
        //  USE Character.DisableAttack()
        //  StartCoroutine(DelayActive());
    }

    // Used for debugging only, attacks are handled in animator
    IEnumerator DelayActive()
    {
        yield return new WaitForSeconds(activeTime);
        if (gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
        }
    }





}
