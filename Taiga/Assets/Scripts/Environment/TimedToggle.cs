using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimedToggle : MonoBehaviour
{
    public float delayTime;
    public UnityEvent toggleEvent;
    public bool manualActivate;
    bool active;
    float timer;

    void Update()
    {
        if(manualActivate && !active){
            manualActivate = false;
            StartTimer();
        }
        if (active)
        {
            timer += Time.deltaTime;
            if (timer > delayTime)  {ActivateToggle();  }
        }
    }

    public void ActivateToggle()
    {
        toggleEvent.Invoke();
        gameObject.SetActive(false);
    }

    public void StartTimer()
    {
        active = true;
        timer = 0;
    }
}


