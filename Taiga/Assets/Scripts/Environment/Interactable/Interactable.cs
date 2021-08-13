using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//  Use on objects that player can interact with
[RequireComponent(typeof(Collider2D))]
public class Interactable : MonoBehaviour
{
    [HideInInspector]
    public Player player;
    public UnityEvent activeEvent, inactiveEvent;
    public bool     active = true,                      // Determines if the Active or Inactive Event triggers on Interact.
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
                if (player.InteractCall)
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
        else if (inactiveEvent.GetPersistentEventCount() > 0)
        {
            inactiveEvent.Invoke();
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

    public void DeactivateOnInteract()
    {
        gameObject.SetActive(false);
    }


    public void PauseTime(){
        GameController.Instance.Pause();
    }

    public void ResumeTime(){
        GameController.Instance.Resume();
    }

    //  Mainly for debugging purpose, can be used to display a string to the Log
    public void DisplayDebug(string message)
    {
        Debug.Log(message);
    }
}
