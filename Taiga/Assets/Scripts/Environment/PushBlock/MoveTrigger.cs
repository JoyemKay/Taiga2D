using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*  Used for detection around a push block object, 
 * tracks if a player is inside collider, 
 * or if it is touching a non-trigger collider*/
public class MoveTrigger : MonoBehaviour
{
    public bool isOccupied, playerOccupied;
    public Vector2 direction;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.isTrigger)
        {
            isOccupied = true;
            if(other.GetComponent<Player>()){
                playerOccupied = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.isTrigger)
        {
            isOccupied = false;
            if(other.GetComponent<Player>()){
                playerOccupied = false;
            }
        }
    }

    public bool IsOccupied(){
        return isOccupied;
    }

}
