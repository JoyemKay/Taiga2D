using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// + + DEPENDS ON GameController.Instance + + //

public class Room : MonoBehaviour
{
    public bool debug;
    public bool isActiveRoom, isDefaultRoom = false;
    public string id, roomName;
    public bool displayRoomName;
    [HideInInspector]
    public BoxCollider2D roomCollider;
    RoomChapter[] roomChapters;
    WorldObject[] roomObjects;
    bool hasStarted;

    void Start()
    {
        if (debug)
        {
            DebugIdCheck();
        }
        hasStarted = true;
        roomCollider = GetComponent<BoxCollider2D>();
    }

    public void Setup()
    {
        roomChapters = GetComponentsInChildren<RoomChapter>();

        Debug.Log("Setup called on room: " + roomName + " (" + "id: " + id + ") ...");

        for (int i = 0; i < roomChapters.Length; i++)
        {
            Debug.Log("Setup called on chapter: " + roomChapters[i].name + "...");
            roomChapters[i].Setup();
        }
    }

    public void EnableChapters()
    {

        for (int i = 0; i < roomChapters.Length; i++)
        {
            roomChapters[i].TryEnableChapter();
        }
    }

    public void DisableChapters()
    {
        for (int i = 0; i < roomChapters.Length; i++)
        {
            roomChapters[i].DisableChapter();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasStarted)
        {
            isActiveRoom = (other.CompareTag("Player") && !other.isTrigger);

            if (isActiveRoom)
            {
                GameController.Instance.ActivateRoom(this, roomCollider);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (hasStarted)
        {
            isActiveRoom = (other.CompareTag("Player") && !other.isTrigger);

            if (!isActiveRoom && other.CompareTag("Player"))
            {
                GameController.Instance.DisableRoom(this);
            }
        }
    }

    #region Debug stuff
    //Checks if there is another Room in the scene with the same ID, the ID should be Scene unique.
    void DebugIdCheck()
    {
        Room[] sceneRooms;
        sceneRooms = FindObjectsOfType<Room>();

        for (int i = 0; i < sceneRooms.Length; i++)
        {
            if (id == sceneRooms[i].id && !(this == sceneRooms[i]))
            {
                Debug.Log("Warning, room: " + gameObject.name + " has the same ID as: " + sceneRooms[i].gameObject.name);
            }
        }
    }
    #endregion
}
