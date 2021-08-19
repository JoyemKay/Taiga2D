using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimedToggle : MonoBehaviour
{
    public float    countdown,          // Time in seconds until event is invoked after timer start 
                    onActiveDelay;      // Time in seconds until event is invoked after gameObject active
    public UnityEvent toggleEvent;
    public bool manualActivate,         // Should the timed event be triggered on gameObject active?
                oneTime;                // Is the timer used only once?
    bool active;
    float timer;

    void Update()
    {
        if(manualActivate && !active){
            manualActivate = false;
            StartCoroutine(Wait(onActiveDelay));
        }
        if (active)
        {
            timer += Time.deltaTime;
            if (timer > countdown)  {ActivateToggle();  }
        }
    }

    IEnumerator Wait(float time){
        yield return new WaitForSeconds(time);
        ActivateToggle();
    }

    public void ActivateToggle()
    {
        toggleEvent.Invoke();
        if (oneTime)
        {
            gameObject.SetActive(false);
        }
    }

    public void StartTimer()
    {
        active = true;
        timer = 0;
    }
}


