using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AClockworkBerry;


public class GameController : MonoBehaviour
{
    public Chapter currentChapter;
    public Player activePlayer;
    public GameObject playerPrefab;
    public bool debugging;
    public static GameController Instance
    {
        get { return _instance; }
    }
    public CameraController GetCameraController
    {
        get { return cameraController; }
    }
    public bool IsPaused
    {
        get { return _isPaused; }
    }


    float currentTimeScale = 1, roomTransitionDelay = 4f;
    bool _isPaused;
    static GameController _instance;
    UiController ui;
    SpawnLocation levelSpawnLocation;
    CameraController cameraController;
    FloatingTextManager floatingTextManager;
    AstarGrid grid;


    //Debugging
    ScreenLogger screenLogger;

    private void Awake()
    {
        #region singleton pattern
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(this.gameObject);
        #endregion
        Random.InitState(System.DateTime.Now.Millisecond);

        if (!grid) { grid = GetComponentInChildren<AstarGrid>(); }
        if (!ui) 
        { 
            ui = GetComponentInChildren<UiController>(); 
            if(!floatingTextManager){
                floatingTextManager = ui.gameObject.GetComponentInChildren<FloatingTextManager>();
                if (!floatingTextManager) { Debug.Log("No FloatingTextManager found..."); }
            }
        }

        if (!screenLogger) { screenLogger = GetComponentInChildren<ScreenLogger>(); }
        if (!cameraController) { cameraController = GetComponentInChildren<CameraController>(); }

        cameraController.Initiate();

        //Sets up all Rooms in the Scene, then disables children Chapters
        Room[] sceneRooms = FindObjectsOfType<Room>();

        Debug.Log("Found " + sceneRooms.Length + " rooms, calling setup...");

        levelSpawnLocation = null;
        for (int i = 0; i < sceneRooms.Length; i++)
        {
            if (sceneRooms[i].isDefaultRoom) { levelSpawnLocation = sceneRooms[i].GetComponentInChildren<SpawnLocation>(); }
            sceneRooms[i].Setup();
        }
        if (!levelSpawnLocation)
        {
            levelSpawnLocation = FindObjectOfType<SpawnLocation>();
            if (!levelSpawnLocation)
            {
                Debug.Log("WARNING: No active spawn location in scene, defaulting to bottom left corner.");
            }
        }

        InstantiatePlayer();
        cameraController.target = activePlayer.gameObject;

        //DEBUGGING
        if (!debugging)
        {
            if (!activePlayer)
            {
                activePlayer = FindObjectOfType<Player>();
            }
            screenLogger.ShowLog = false;
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Pause key pressed!");
            if (IsPaused)
            {
                Resume();
            }
            else
            {
                Pause();

            }
            ui.PauseScreen();
        }

        //DEBUGGING
        if (debugging)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                screenLogger.ShowLog = !screenLogger.ShowLog;
            }
        }
    }

    #region game time related
    public void Pause()
    {

        currentTimeScale = Time.timeScale;
        Time.timeScale = 0;
        _isPaused = true;
    }

    public void Resume()
    {
        Time.timeScale = currentTimeScale;
        _isPaused = false;
    }
    #endregion

    public void ChangeLevel()
    {
        //TODO: Transition to level stated in transition variable. Do level exit stuff (save and clear memory).
        activePlayer.gameObject.SetActive(false);
        Debug.Log("Changing level...");
    }

    //Changes the current chapter of the game, and scene. Enables the Chapter in the active Room.
    public void SetChapter(Chapter chapter)
    {
        currentChapter = chapter;

        Room[] sceneRooms = FindObjectsOfType<Room>();
        for (int i = 0; i < sceneRooms.Length; i++)
        {
            if (sceneRooms[i].isActiveRoom)
            {
                sceneRooms[i].EnableChapters();
            }
        }
    }

    public void InstantiatePlayer()
    {
        Vector2 spawnLocation = new Vector2();
        if (!levelSpawnLocation)
        {
            spawnLocation = transform.position;
        }
        else
        {
            spawnLocation = levelSpawnLocation.transform.position;
        }

        GameObject playerObject = Instantiate(playerPrefab, spawnLocation, Quaternion.identity);
        playerObject.name = "PlayerContainer";
        activePlayer = playerObject.GetComponent<Player>();
        activePlayer.lookDirection = levelSpawnLocation.GetDirection();

    }

    public void LinkPlayer(Player player)
    {
        activePlayer = player;
        SetupPlayer();
    }

    void SetupPlayer()
    {

    }

    public void ActivateRoom(Room activeRoom, BoxCollider2D roomCollider)
    {
        if (grid)
        {
            cameraController.SetBoundaries(roomCollider);
            Vector2 pos = roomCollider.offset;
            grid.transform.position = pos;
            grid.gridWorldSize = new Vector2(roomCollider.size.x, roomCollider.size.y);
        }

        Debug.Log("Activating Room: " + activeRoom.name);
        ui.TryDisplayRoomName(activeRoom);
        activeRoom.EnableChapters();
    }

    public void DisableRoom(Room activeRoom)
    {

        Debug.Log("Disabling Room: " + activeRoom.name);
        activeRoom.DisableChapters();
    }

    public void TransitionPlayer(RoomTransition transition, bool levelTransition)
    {
        StartCoroutine(TransitionDelay(transition));
    }

    IEnumerator TransitionDelay(RoomTransition transition)
    {
        //Black out the screen here
        Debug.Log("Transition is to new level: " + transition.isLevelTransition);
        Pause();
        //Room fades in and out over the duration of roomTransitionDelay.
        ui.FadeRoom(roomTransitionDelay / 2, roomTransitionDelay / 2);
        yield return new WaitForSecondsRealtime(roomTransitionDelay / 2);

        Resume();
        //Moves player to transition point if not levelTransition
        if (!transition.isLevelTransition) { transition.Transition(); }
        else
        {
            ChangeLevel();
            yield break;
        }
        //New room won't trigger while game is paused
        yield return null;

        yield return null;

        //Continue with pause for the duration of roomTransitionDelay.
        Pause();
        yield return new WaitForSecondsRealtime(roomTransitionDelay / 4);

        Resume();
        if (transition.isLevelTransition) { ChangeLevel(); }
    }

    #region floating text related
    public void ShowFloatingText(string msg, int fontSize, Color color, Vector3 position, Vector3 motion, float duration){
        floatingTextManager.Show(msg, fontSize, color, position, motion, duration);
    }
    #endregion
}

