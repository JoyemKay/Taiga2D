using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class that contains item information for item gameObject
public class ItemContainer : MonoBehaviour
{
    public Item item;

    public void Start()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (item) { renderer.sprite = item.itemSprite; }
    }

    public void Pickup(){
        // Pick up item to inventory
        Debug.Log("Picking up "+gameObject.name);
        //TODO: add item to player inventory or whatever
    }

}
