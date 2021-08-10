using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{

    public void LoadNewScene(string sceneName){
        GameController.Instance.ChangeLevel(sceneName);
    }

}
