using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//  Use on objects that player can interact with
[RequireComponent(typeof(Collider2D))]
public class Interactable : MonoBehaviour
{
    Player player;
    public UnityEvent activeEvent, disabledEvent;
    public bool     active = true,                      // Determines if the Active or Disabled Event triggers on Interact.
                    interactOnTriggerEnter,             // Should the Interact happen on the player entering trigger collider?
                    oneTime,                            // Should the Interact only be able to happen once
                    deactivateOnInteract,               // Should the Object be disabled on Interact?
                    interactableByPlayerInput = true;   // Should the player be able to Interact by object trigger collider?

    bool hasInteracted, playerInRange;

    public void Update()
    {
        if (interactableByPlayerInput)
        {
            if (playerInRange && player)
            {
                if (Input.GetButtonDown("Interact") && player.canInteract)
                {
                    Interact();
                }
            }
        }
    }

    public void Interact()
    {
        //  Only invoke interactableEvent if active
        if (active)
        {
            //Only invoke one time event once
            if (oneTime && hasInteracted)
            {
                return;
            }

            activeEvent.Invoke();
            hasInteracted = true;
            if (deactivateOnInteract)
            {
                DeactivateOnInteract();
            }

        }
        //  Invoke disabledEvent if there are listerners registered
        else if (disabledEvent.GetPersistentEventCount() > 0)
        {
            disabledEvent.Invoke();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Player>())
        {
            if (other.GetComponent<Player>().interactCollider == other)
            {
                playerInRange = true;
                player = other.GetComponent<Player>();
                if(interactOnTriggerEnter){
                    Interact();
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Player>())
        {
            if (other.GetComponent<Player>().interactCollider == other)
            {
                playerInRange = false;
                player = null;
            }
        }
    }

    //Toggles the active state
    public void ToggleActiveState(){
        active = !active;
    }
    /*
    public void SetActivated()
    {
        active = true;
    }

    public void SetDeactivated()
    {
        active = false;
    }
    */
    public void DeactivateOnInteract()
    {
        gameObject.SetActive(false);
    }


    //  Mainly for debugging purpose, can be used to display a string to the Log
    public void DisplayDebug(string message)
    {
        Debug.Log(message);
    }
}
