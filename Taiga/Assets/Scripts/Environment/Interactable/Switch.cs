using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//  Use on objects that should switch between an active and deactivated state via animator
[RequireComponent(typeof(Animator))]
public class Switch : MonoBehaviour
{
    public bool switchActive;
    Animator switchAnimator;
    /*
    private void Start()
    {
        switchAnimator = GetComponent<Animator>();
        switchAnimator.SetBool("switch", switchActive);
    }
    */

    public void ToggleSwitch()
    {
        switchAnimator.SetBool("switch", !switchActive);
        switchActive = !switchActive;
    }

    private void OnEnable()
    {
        switchAnimator = GetComponent<Animator>();
        switchAnimator.SetBool("switch", switchActive);
    }
}
