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
    bool okToFadeIn, fadingInAfterSceneLoad;

    public void TryDisplayRoomName(Room room)
    {
        if (RoomLocationText.enabled)
        {
            StopCoroutine(RoomNameFader());
        }
        if (room.displayRoomName)
        {
            RoomLocationText.text = room.roomName;
            RoomLocationText.enabled = true;
            StartCoroutine(RoomNameFader());
        }
    }

    public void FadeRoom(float fadeOutTime, float fadeInTime)
    {
        StartCoroutine(FadeOut(fadeOutTime));
        StartCoroutine(FadeIn(fadeInTime));
    }

    IEnumerator RoomNameFader()
    {
        yield return new WaitForSecondsRealtime(roomNameFadeOutTime);
        RoomLocationText.enabled = false;
    }

    IEnumerator FadeOut(float time)
    {
        RoomFader.enabled = true;
        float fadeTime = Time.unscaledTime;
        Color fadeColor;
        while (Time.unscaledTime < (fadeTime + time))
        {
            fadeColor = new Color(RoomFader.color.r, RoomFader.color.g, RoomFader.color.b, 1 - ((fadeTime + time - Time.unscaledTime) / time));
            RoomFader.color = fadeColor;
            yield return null;
        }

        fadeColor = new Color(RoomFader.color.r, RoomFader.color.g, RoomFader.color.b, 1);
        RoomFader.color = fadeColor;
        Debug.Log(Time.frameCount + ": 2. Fade out complete, fader alpha-value is: " + fadeColor.a);

        okToFadeIn = true;
    }

    IEnumerator FadeInAfterLevelLoad(float time, float holdTime){
        yield return new WaitForSecondsRealtime(holdTime);
        while (!GameController.Instance.SceneFinishedLoading)
        {
            yield return null;
        }
        yield return null;
        yield return null;
        fadingInAfterSceneLoad = true;
        StartCoroutine(FadeIn(time));
    }

    IEnumerator FadeIn(float time)
    {
        while (!okToFadeIn)
        {
            yield return null;
        }

        float fadeTime = Time.unscaledTime;
        Color fadeColor;
        while (Time.unscaledTime < (fadeTime + time))
        {
            fadeColor = new Color(RoomFader.color.r, RoomFader.color.g, RoomFader.color.b, ((fadeTime + time - Time.unscaledTime) / time));
            RoomFader.color = fadeColor;
            yield return null;
        }

        fadeColor = new Color(RoomFader.color.r, RoomFader.color.g, RoomFader.color.b, 0);
        RoomFader.color = fadeColor;
        Debug.Log(Time.frameCount + ": 6. Fade in complete, fader alpha-value is: " + fadeColor.a);
        yield return null;

        RoomFader.enabled = false;
        okToFadeIn = false;
    }

    public void PauseScreen()
    {
        pauseScreen.SetActive(!pauseScreen.activeInHierarchy);
    }

    public void SceneTransitionFader(float fadeInAndOutTime, float holdDelay){
        fadingInAfterSceneLoad = false;
        StartCoroutine(FadeOut(fadeInAndOutTime / 2));
        StartCoroutine(FadeInAfterLevelLoad(fadeInAndOutTime / 2, holdDelay));
        StartCoroutine(DelayLoadingScreen(fadeInAndOutTime / 2));
    }

    IEnumerator DelayLoadingScreen(float fadeTime)
    {
        yield return new WaitForSecondsRealtime(fadeTime);
        Debug.Log(Time.frameCount + ": 3U. Fade out time passed, resuming time for level load");
        GameController.Instance.Resume();

        Debug.Log(Time.frameCount + ": Continuing in DelayLoadingScreen...");

        while(!fadingInAfterSceneLoad){
            yield return null;
        }
        Debug.Log(Time.frameCount + ": Continuing in DelayLoadingScreen...");

        //yield return new WaitForSecondsRealtime(holdDelay/2);
        Debug.Log(Time.frameCount + ": 5. Pausing game during delay and fade in...)");
        GameController.Instance.Pause();

        while(okToFadeIn){
            yield return null;
        }
        //yield return new WaitForSecondsRealtime(holdDelay/2+fadeTime);
        Debug.Log(Time.frameCount + ": 7. Fade in time has passed, resuming game...");
        GameController.Instance.Resume();
    }
}
