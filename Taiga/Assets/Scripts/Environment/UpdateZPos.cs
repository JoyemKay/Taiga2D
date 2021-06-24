using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateZPos : MonoBehaviour
{
    WorldObject[] objects;
    void Start()
    {
        objects = GetComponentsInChildren<WorldObject>();
        for (int i = 0; i < objects.Length; i++)
        {
                Vector3 objPos = objects[i].transform.position;
                Vector3 pos =  new Vector3(objPos.x, objPos.y, objPos.y+objects[i].offset);
                objects[i].transform.position = pos;
        }
    }
}
