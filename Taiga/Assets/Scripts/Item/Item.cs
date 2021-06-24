using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* Scriptable object for items in game.
*/
[CreateAssetMenu(fileName = "New item",menuName = "ScriptableObjects/Item")]
public class Item : ScriptableObject
{
    public string itemName,itemDescription,itemID;
    public Sprite itemSprite;

}
