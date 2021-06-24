using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingText
{
    public bool active;
    public GameObject go;
    public Text thisText;
    public Vector3 motion;
    public float duration, lastShown;

    private Camera currentCamera;
    private Vector3 cameraZeroPos;

    public void Show()
    {
        active = true;
        lastShown = Time.time;
        currentCamera = GameController.Instance.GetCameraController.mainCamera;
        cameraZeroPos = currentCamera.ScreenToWorldPoint(currentCamera.transform.position);
        go.SetActive(active);
    }

    public void Hide()
    {
        active = false;
        currentCamera = null;
        cameraZeroPos = Vector3.zero;
        go.SetActive(active);
    }

    public void UpdateFloatingText(){
        if (!active) return;

        if(Time.time - lastShown > duration){
            Hide();
        }

        //FloatingText object stays in worldspace, regardless of camera movement
        go.transform.position += (cameraZeroPos - currentCamera.ScreenToWorldPoint(currentCamera.transform.position)) + motion * Time.deltaTime;
        cameraZeroPos = currentCamera.ScreenToWorldPoint(currentCamera.transform.position);
    }
}
