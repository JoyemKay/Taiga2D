using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public GameObject target;
    BoxCollider2D roomBoundaries;
    public Camera mainCamera;

    public void Initiate()
    {
        mainCamera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            Vector3 targetPos = target.transform.position;

            Vector3 pos;
            if (roomBoundaries)
            {
                float localX;
                float localY;

                if(roomBoundaries.bounds.size.x < mainCamera.orthographicSize*mainCamera.aspect*2){
                    localX = roomBoundaries.bounds.center.x;
                }else{
                    localX = Mathf.Clamp(targetPos.x, roomBoundaries.bounds.min.x + mainCamera.orthographicSize * mainCamera.aspect, roomBoundaries.bounds.max.x - mainCamera.orthographicSize * mainCamera.aspect);
                }

                if (roomBoundaries.bounds.size.y < mainCamera.orthographicSize * 2)
                {
                    localY = roomBoundaries.bounds.center.y;
                }
                else
                {
                    localY = Mathf.Clamp(targetPos.y, roomBoundaries.bounds.min.y + mainCamera.orthographicSize, roomBoundaries.bounds.max.y - mainCamera.orthographicSize);
                }

                pos = new Vector3(localX,
                                  localY,
                                  transform.position.z);

            }
            else
            {
                pos = new Vector3(targetPos.x,
                                  targetPos.y,
                                  transform.position.z);
            }

            transform.position = pos;
        }
    }

    public void SetBoundaries(BoxCollider2D collider)
    {
        roomBoundaries = collider;
    }
}
