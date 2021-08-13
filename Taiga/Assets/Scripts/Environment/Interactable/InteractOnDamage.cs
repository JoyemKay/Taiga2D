using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InteractOnDamage : MonoBehaviour
{
    Interactable interactable;
    Animator animator;
    Collider2D blockingCollider;

    private void Start()
    {
        if (!interactable)
        {
            interactable = GetComponent<Interactable>();
        }
        animator = GetComponent<Animator>();
        Collider2D[] colliders = GetComponents<Collider2D>();
        for (int i = 0; i < colliders.Length;i++){
            if(!colliders[i].isTrigger){
                blockingCollider = colliders[i]; 
            }
        }
        animator.enabled = false;
    }

   /* private void OnTriggerEnter2D(Collider2D other)
    {
        //TODO: Generalize to all damage (melee and projectile)
        if(other.gameObject.layer == LayerMask.NameToLayer("Player Damage"))    {interactable.Interact();   }
    }*/

    public void Interact(){
        interactable.Interact();
    }

    public void DestroyOnInteract(){
        animator.enabled = true;
        blockingCollider.enabled = false;
        animator.SetTrigger("destroy");
    }

    // Called from animator
    public void Destroy()
    {
        GetComponent<WorldObject>().DisableWorldObject();
    }

    private void OnEnable()
    {
        animator.enabled = false;
        blockingCollider.enabled = true;
    }
}
