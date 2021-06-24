using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Destructable : MonoBehaviour
{
    public UnityEvent OnDestroyEvent;
    Collider2D thisCollider;
    Animator thisAnimator;

    //  Needs to be called before WordObject.Start(), otherwise randomizing of sprite is overwritten by animator.
    //
    void Awake(){
        thisAnimator = GetComponent<Animator>();
        thisCollider = GetComponent<Collider2D>();
        thisAnimator.enabled = false;
    }

    //  Triggers the destroy animation and destroys object, is called from Damage object on hit.
    //
    public void StartDestroyAnimator(){
        thisAnimator.enabled = true;
        thisCollider.enabled = false;
        thisAnimator.SetTrigger("destroy");
    }

    //  If something is suppose to happen when object is destroyed, it's invoked
    // via OnDestroyEvent (loot drop, opening of door etc.)
    public void Destroy(){
        OnDestroyEvent.Invoke();
        GetComponent<WorldObject>().DisableWorldObject();
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        thisAnimator.enabled = false;
        thisCollider.enabled = true;
    }
}