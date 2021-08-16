using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetGameTime : MonoBehaviour
{

    public void Pause(){
        GameController.Instance.Pause();
    }

    public void Resume(){
        GameController.Instance.Resume();
    }
}
