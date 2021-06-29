using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTransit : MonoBehaviour
{
    public int fromFloor, toFloor;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.isTrigger)
            return;
        Character character = other.GetComponent<Character>();
        if (!character)
            return;
        if (character.GetFloor() == fromFloor)
        {
            Debug.Log("Changing floor from: " + fromFloor + " to: " + toFloor + " for: " + character.name);
            character.SetFloor(toFloor);
        }
    }
}
