using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemSpawner : MonoBehaviour
{
    Item item;
    public GameObject itemPrefab;
    public ItemSpawnStruct[] itemList;

    Item CalculateItem(){
        float totalChance = 0;

        for (int i = 0; i < itemList.Length;i++){
            totalChance += itemList[i].chance;
        }

        float roll = Random.Range(0, totalChance);
        Debug.Log("Roll: " + roll);
        float accChance = 0;

        for (int i = 0; i < itemList.Length; i++)
        {
            
            accChance += itemList[i].chance;
            if(roll < accChance){
                if (itemList[i].item)
                {
                    Debug.Log("Item roll result: " + itemList[i].item.itemName);
                }else{
                    Debug.Log("No item rolled.");
                }
                return itemList[i].item;
            }
        }

        return null;
    }

    public void SpawnItem(){
        item = CalculateItem();
        Debug.Log("Trying to spawn item...");
        if(item != null){
            StartCoroutine(CoSpawnItem(item, transform));
        }
    }

    IEnumerator CoSpawnItem(Item _item, Transform _transform){
        GameObject instantiatedItem = Instantiate(itemPrefab, _transform.position, _transform.rotation);
        ItemContainer itemContainer = instantiatedItem.GetComponent<ItemContainer>();
        itemContainer.item = _item;
        yield break;
    }
}
