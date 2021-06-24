using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

}
