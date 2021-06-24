using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiController : MonoBehaviour
{
    public Text RoomLocationText;
    public Image RoomFader;
    public GameObject pauseScreen;
    public float roomNameFadeOutTime;
    bool okToFadeIn;

    public void TryDisplayRoomName(Room room){
        if(RoomLocationText.enabled){
            StopCoroutine(RoomNameFader());
        }
        if(room.displayRoomName){
            RoomLocationText.text = room.roomName;
            RoomLocationText.enabled = true;
            StartCoroutine(RoomNameFader());
        }
    }

    public void FadeRoom(float fadeOut,float fadeIn){
        StartCoroutine(FadeOut(fadeOut)); 
        StartCoroutine(FadeIn(fadeIn)); 
    }

    IEnumerator RoomNameFader(){
        yield return new WaitForSecondsRealtime(roomNameFadeOutTime);
        RoomLocationText.enabled = false;
    }

    IEnumerator FadeOut(float time){
        RoomFader.enabled = true;
        float fadeTime = Time.unscaledTime;
        while(Time.unscaledTime < (fadeTime + time)){
            Color fadeColor = new Color(RoomFader.color.r, RoomFader.color.g, RoomFader.color.b, 1 - ((fadeTime + time - Time.unscaledTime) / time));
            RoomFader.color = fadeColor;
            yield return null;
        }
        okToFadeIn = true;
    }

    IEnumerator FadeIn(float time){
        while(!okToFadeIn){
            yield return null;
        }
        float fadeTime = Time.unscaledTime;
        while (Time.unscaledTime < (fadeTime + time))
        {
            Color fadeColor = new Color(RoomFader.color.r, RoomFader.color.g, RoomFader.color.b, ((fadeTime + time - Time.unscaledTime) / time));
            RoomFader.color = fadeColor;
            yield return null;
        }
        RoomFader.enabled = false;
        okToFadeIn = false;
    }

    public void PauseScreen(){
        pauseScreen.SetActive(!pauseScreen.activeInHierarchy);
    }
}
