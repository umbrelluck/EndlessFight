using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    //public Transform text;
    public TextMeshProUGUI fpsText;
    void Start()
    {
        //fpsText = text.gameObject.GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        float fps = 1 / Time.unscaledDeltaTime;
        fpsText.text = "" + fps;
    }
}
