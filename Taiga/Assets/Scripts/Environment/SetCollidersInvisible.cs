using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Disables the TilemapRenderer component for Tilemap objects tagged with 'MapCollider'
// Colliders are painted on the scene in the two collider tilemaps 'Collision' and 'Collision-platform_down'

public class SetCollidersInvisible : MonoBehaviour
{
    TilemapRenderer[] collisionRenderers;
    bool renderersDisabled;
    void Start()
    {
        if (!renderersDisabled)
        {
            collisionRenderers = GetComponentsInChildren<TilemapRenderer>();
            for (int i = 0; i < collisionRenderers.Length; i++)
            {
                if (collisionRenderers[i].gameObject.CompareTag("MapCollider")) { collisionRenderers[i].enabled = false; }
            }
            renderersDisabled = true;
        }
    }
}
