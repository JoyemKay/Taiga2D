using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using AClockworkBerry;


public class GameController : MonoBehaviour
{
    public Chapter currentChapter;
    public Player activePlayer;
    public GameObject playerPrefab;
    GameObject sessionPlayerObject;
    bool hasSessionPlayer;

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
    public bool SceneFinishedLoading{
        get { return _sceneFinishedLoading; }
    }
    public string CachedSpawnLocation
    {
        set { _cachedSpawnLocation = value; }
    }

    float currentTimeScale = 1, roomTransitionDelay = 4f;
    bool _isPaused, sceneInstantiated, _sceneFinishedLoading;
    string _cachedSpawnLocation;
    static GameController _instance;
    UiController ui;
    SpawnLocation levelSpawnLocation;
    CameraController cameraController;
    FloatingTextManager floatingTextManager;
    AstarGrid grid;


    //Debugging
    ScreenLogger screenLogger;
    public float sceneLoadDelayTime, blackOutDelay;

    private void OnEnable()
    {
        Debug.Log("Game Controller " + gameObject.GetInstanceID() + " was enabled.");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode){
        TryInstantiateScene();
    }

    private void Awake()
    {
        Debug.Log("Awake call on " + name + ",ID: "+ gameObject.GetInstanceID());
        #region singleton pattern
        if (_instance != null && _instance != this)
        {
            Debug.Log("Duplicate of singleton " + name + " detected, terminating copy...");
            Destroy(this.gameObject);
            return;
        }
        if (_isPaused != this)
        {
            _instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
        #endregion

        Random.InitState(System.DateTime.Now.Millisecond);

        Debug.Log("Time scale is: " + Time.timeScale);

        if (!grid) { grid = GetComponentInChildren<AstarGrid>(); }
        if (!ui)
        {
            ui = GetComponentInChildren<UiController>();
            if (!floatingTextManager)
            {
                floatingTextManager = ui.gameObject.GetComponentInChildren<FloatingTextManager>();
                if (!floatingTextManager) { Debug.Log("No FloatingTextManager found..."); }
            }
        }

        if (!screenLogger) { screenLogger = GetComponentInChildren<ScreenLogger>(); }
        if (!cameraController) { cameraController = GetComponentInChildren<CameraController>(); }

        cameraController.Initiate();

        TryInstantiateScene();


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

        //TODO: Should be in UiController
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

    public void ChangeLevel(string sceneName)
    {
        //TODO: Transition to level stated in transition variable. Do level exit stuff (save and clear memory).
        //TODO: Save values of player object, reinstatiate with that instead of prefab
        _sceneFinishedLoading = false;
        Pause();
        ui.SceneTransitionFader(sceneLoadDelayTime, blackOutDelay);
        Debug.Log(Time.frameCount + ": 1. Changing level...");
        StartCoroutine(AsyncSceneLoad(sceneName, sceneLoadDelayTime/2));
    }

    IEnumerator AsyncSceneLoad(string sceneName, float delay){
        yield return new WaitForSecondsRealtime(delay);
        Debug.Log(Time.frameCount + ": 3G. Fade out time has passed, loading new level...");
        activePlayer.transform.parent = transform;
        activePlayer.gameObject.SetActive(false);
        sceneInstantiated = false;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while(!asyncLoad.isDone){
            yield return null;
        }
        _sceneFinishedLoading = true;
        Debug.Log(Time.frameCount + ": 4. Level has finished loading...");
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

    private void TryInstantiateScene(){
        Debug.Log("Trying to instantiate scene...");
        if (!sceneInstantiated)
        {
            Debug.Log("Scene is not instantiated, proceeding...");
            //Sets up all Rooms in the Scene, then disables children Chapters
            Room[] sceneRooms = FindObjectsOfType<Room>();

            Debug.Log("Found " + sceneRooms.Length + " rooms, calling setup...");

            levelSpawnLocation = null;
            SpawnLocation defaultSpawn =null;
            for (int i = 0; i < sceneRooms.Length; i++)
            {
                if (!levelSpawnLocation)
                {
                    if (sceneRooms[i].isDefaultRoom) {
                        Debug.Log("Found default spawn location ...");
                        defaultSpawn = sceneRooms[i].defaultSpawnLocation; 
                    }
                    if (_cachedSpawnLocation != null)
                    {
                        SpawnLocation[] spawnLocations = sceneRooms[i].gameObject.GetComponentsInChildren<SpawnLocation>();
                        for (int j = 0; j < spawnLocations.Length; j++){
                            if (spawnLocations[j].spawnName != null)
                            {
                                Debug.Log("Testing spawnName: " + spawnLocations[j].spawnName + " against cached string: " + _cachedSpawnLocation);
                                if (spawnLocations[j].spawnName.Equals(_cachedSpawnLocation))
                                {
                                    Debug.Log("Found target spawn location from previous scene, initialising ...");
                                    levelSpawnLocation = spawnLocations[j];
                                    _cachedSpawnLocation = null;
                                }
                            }
                        }
                    }
                }
                sceneRooms[i].Setup();
            }
            if (_cachedSpawnLocation != null)
            {
                // If a target spawn location was transfered from previous scene, but not found in any Room, reset and call out.
                Debug.Log("Target spawn location was not found in scene, reverting to default ...");
                _cachedSpawnLocation = null;
            }

            if (!levelSpawnLocation)
            {

                if(defaultSpawn){
                    Debug.Log("Spawing at default spawn position ...");
                    levelSpawnLocation = defaultSpawn;
                }
                if (!levelSpawnLocation)
                {
                    Debug.Log("No default spawn point found, searching for first available spawn point...");
                    levelSpawnLocation = FindObjectOfType<SpawnLocation>();

                    if (!levelSpawnLocation)
                    {
                        Debug.Log("WARNING: No active spawn location in scene, defaulting to bottom left corner.");
                    }
                }
            }
            sceneInstantiated = true;

            InstantiatePlayer();
            cameraController.target = activePlayer.gameObject;

            return;
        }
        Debug.Log("Scene was already instantiated, returning.");
    }

    private void InstantiatePlayer()
    {
        Debug.Log("Instantiating player object...");
        Vector2 spawnLocation = new Vector2();
        if (!levelSpawnLocation)
        {
            Debug.Log("No Level spawn location found, spawning at origin.");

            spawnLocation = transform.position;
        }
        else
        {
            spawnLocation = levelSpawnLocation.transform.position;
        }
        GameObject playerObject;
        if (!hasSessionPlayer)
        {
            Debug.Log("No player object in hierarchy, instantiating player prefab...");
            playerObject = Instantiate(playerPrefab, spawnLocation, Quaternion.identity);
            playerObject.name = "Player";
            activePlayer = playerObject.GetComponent<Player>();
            hasSessionPlayer = true;
        }else{
            Debug.Log("Active player found, continuing with player in hierarchy...");
            activePlayer.transform.parent = null;
            playerObject = activePlayer.gameObject;
            playerObject.transform.position = spawnLocation;
            SceneManager.MoveGameObjectToScene(playerObject, SceneManager.GetActiveScene());
        }

        activePlayer.Initiate(levelSpawnLocation.floor);
        activePlayer.lookDirection = GetDirection(levelSpawnLocation.lookDirection);

    }

    //Converts from struct Direction into Vector2
    static public Vector2 GetDirection(Direction lookDirection)
    {
        Vector2 direction;
        switch (lookDirection)
        {
            case Direction.up:
                direction = Vector2.up;
                break;
            case Direction.down:
                direction = Vector2.down;
                break;
            case Direction.left:
                direction = Vector2.left;
                break;
            case Direction.right:
                direction = Vector2.right;
                break;
            default:
                direction = Vector2.zero;
                break;
        }
        return direction;
    }

    public void LinkPlayer(Player player)
    {
        activePlayer = player;
        SetupPlayer();
    }

    void SetupPlayer()
    {
        //TODO: get stats from playerStats
    }

    public void SetColliderLayer(GameObject callObject, int layerCode)
    {

        string layer = "Collision_" + layerCode.ToString();
        Debug.Log("Layer change called on " + callObject.name + ", changing layer from: " + callObject.layer + " to: " + LayerMask.NameToLayer(layer));
        callObject.layer = LayerMask.NameToLayer(layer);
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


    //It feels like this should be under RoomTransition, maybe scene changes are not to be triggered with RoomTransition?
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
            ChangeLevel(transition.levelToLoad);
            yield break;
        }
        //New room won't trigger while game is paused
        yield return null;

        yield return null;

        //Continue with pause for the duration of roomTransitionDelay.
        Pause();
        yield return new WaitForSecondsRealtime(roomTransitionDelay / 4);

        Resume();
        //  if (transition.isLevelTransition) { ChangeLevel(transition.levelToLoad); }
    }

    #region floating text related
    public void ShowFloatingText(string msg, int fontSize, Color color, Vector3 position, Vector3 motion, float duration)
    {
        floatingTextManager.Show(msg, fontSize, color, position, motion, duration);
    }
    #endregion
}

