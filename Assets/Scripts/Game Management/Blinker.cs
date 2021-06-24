using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blinker : MonoBehaviour
{
    public float blinkTime, showTime;
    float timer;
    Text pauseText;


    // Update is called once per frame
    void Update()
    {
        if(timer > Time.unscaledTime - showTime){
            pauseText.enabled = true;
        }else if(timer > Time.unscaledTime - showTime - blinkTime){
            pauseText.enabled = false;
        }else{
            timer = Time.unscaledTime;
        }
    }

    private void OnEnable()
    {
        if (!pauseText) { pauseText = GetComponent<Text>(); }
        timer = Time.unscaledTime;
    }
}
