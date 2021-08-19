using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLocation : MonoBehaviour
{
    SpriteRenderer thisRenderer;
    public int floor;
    public Direction lookDirection;
    public string spawnName;

    private void Awake()
    {
        thisRenderer = GetComponent<SpriteRenderer>();
        thisRenderer.enabled = false;
    }
}
