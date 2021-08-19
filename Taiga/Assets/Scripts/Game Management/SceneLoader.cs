using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    public string targetSpawnLocation;
    public void LoadNewScene(string sceneName){
        GameController.Instance.CachedSpawnLocation = targetSpawnLocation;
        GameController.Instance.ChangeLevel(sceneName);
    }

}
