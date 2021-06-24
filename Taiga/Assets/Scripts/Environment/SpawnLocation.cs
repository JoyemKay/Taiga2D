using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLocation : MonoBehaviour
{
    [SerializeField] SpriteRenderer renderer;
    Vector2 direction;
    public Direction lookDirection;

    private void Awake()
    {
        renderer.enabled = false;
    }

    public Vector2 GetDirection(){
        switch(lookDirection){
            case Direction.up:
                direction = Vector2.up;
                break;
            case Direction.down:
                direction = Vector2.down;
                break;
            case Direction.left:
                direction = Vector2.left;
                break;
            case Direction.right:
                direction = Vector2.right;
                break;
            default:
                direction = Vector2.zero;
                break;
        }
        return direction;
    }

}
