using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//  Generic class for trigger colliders that invokes a damage reaction on target
//  Layer on damage object dictates what other objects can be interacted with
public class Damage : MonoBehaviour{
    float activeTime = 0.1f;
    bool hasHit;
    [SerializeField]
    float damage;


    private void OnTriggerEnter2D(Collider2D other){
        // Only deal damage to first valid target hit
        if (!hasHit){
            //  If colliding collider is a destructable, 
            //  start objects destroy animation, then disable damage object
            Destructable destructable = other.gameObject.GetComponent<Destructable>();
            if (destructable){
                destructable.StartDestroyAnimator();
                hasHit = true;
                gameObject.SetActive(false);
                return;
            }

            //TODO: Check if target is damagable, if so, deal damage
        }
    }

    // Used for debugging only, attacks are handled in animator
    IEnumerator DelayActive(){
        yield return new WaitForSeconds(activeTime);
        if (gameObject.activeInHierarchy){
            gameObject.SetActive(false);
        }
    }

    //  Resets the hasHit variable when object is enabled
    //      
    private void OnEnable(){
        hasHit = false;
        //DEBBUGING ONLY, state should be set in animator
        StartCoroutine(DelayActive());
    }
}
