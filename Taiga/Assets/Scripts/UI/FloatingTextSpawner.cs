using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTextSpawner : MonoBehaviour
{
    public string msg = "default";
    public int fontSize = 25;
    public Vector2 motion = Vector2.up;
    public int motionLength = 200;
    public Color color = Color.white;
    public float duration = 1.0f;

    public void TriggerFloatingText(){
        
        GameController.Instance.ShowFloatingText(msg,fontSize,color,transform.position,motion*motionLength,duration);
    }
}
