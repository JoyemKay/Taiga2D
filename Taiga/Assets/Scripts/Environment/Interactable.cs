using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//  Use on objects that player can interact with
[RequireComponent(typeof(Collider2D))]
public class Interactable : MonoBehaviour
{
    public int floor = 0;
    Player player;
    public UnityEvent interactableEvent, disabledEvent;
    public bool active = true, oneTime, deactivateOnInteract, playerInRange, interactableByPlayerInput = true;

    bool hasInteracted;

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

            interactableEvent.Invoke();
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

    public void SetActivated()
    {
        active = true;
    }

    public void SetDeactivated()
    {
        active = false;
    }

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
