using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingTextManager : MonoBehaviour
{

    public GameObject textContainer, textPrefab;

    private List<FloatingText> floatingTexts = new List<FloatingText>();

    private void Update()
    {
        foreach (FloatingText t in floatingTexts)
            t.UpdateFloatingText();
    }

    public void Show(string msg, int fontSize, Color color, Vector3 position, Vector3 motion, float duration){
        FloatingText floatingText = GetFloatingText();
        floatingText.thisText.text = msg;
        floatingText.thisText.fontSize = fontSize;
        floatingText.thisText.color = color;

        floatingText.go.transform.position = GameController.Instance.GetCameraController.mainCamera.WorldToScreenPoint(position);
        floatingText.motion = motion;
        floatingText.duration = duration;

        floatingText.Show();
    }

    private FloatingText GetFloatingText() {

        FloatingText text = floatingTexts.Find(t => !t.active);

        if(text != null){
            text = new FloatingText();
            text.go = Instantiate(textPrefab);
            text.go.transform.SetParent(textContainer.transform);
            text.thisText = text.go.GetComponent<Text>();

            floatingTexts.Add(text);
        }

        return text;
    }

}

