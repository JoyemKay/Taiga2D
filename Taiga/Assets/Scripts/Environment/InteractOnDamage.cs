using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InteractOnDamage : MonoBehaviour
{
    Interactable interactable;

    private void Start()
    {
        if(!interactable){
            interactable = GetComponent<Interactable>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //TODO: Generalize to all damage (melee and projectile)
        if(other.gameObject.layer == LayerMask.NameToLayer("Player Damage"))    {interactable.Interact();   }
    }
}
