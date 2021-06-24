using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//  Used when an action is suppose to happen on contact with this block, (trap-tile, teleport, etc)
[RequireComponent(typeof(Interactable))]
public class TriggerTile : MonoBehaviour
{
    public MonoBehaviour[] triggerClasses;
    Interactable interactable;


    private void Start()
    {
        interactable = GetComponent<Interactable>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.isTrigger)
        {
            for (int i = 0; i < triggerClasses.Length; i++)
            {
                if (triggerClasses[i])
                {
                    if (collision.gameObject.CompareTag(triggerClasses[i].tag))
                    {
                        interactable.Interact();
                        return;
                    }
                }
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.isTrigger)
        {
            for (int i = 0; i < triggerClasses.Length; i++)
            {
                if (triggerClasses[i])
                {
                    if (collision.gameObject.CompareTag(triggerClasses[i].tag))
                    {
                        interactable.Interact();
                        return;
                    }
                }
            }
        }
    }

}
