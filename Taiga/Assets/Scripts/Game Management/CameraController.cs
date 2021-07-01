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
                pos = new Vector3(Mathf.Clamp(targetPos.x, roomBoundaries.bounds.min.x + mainCamera.orthographicSize * mainCamera.aspect, roomBoundaries.bounds.max.x - mainCamera.orthographicSize * mainCamera.aspect),
                                  Mathf.Clamp(targetPos.y, roomBoundaries.bounds.min.y + mainCamera.orthographicSize, roomBoundaries.bounds.max.y - mainCamera.orthographicSize),
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
