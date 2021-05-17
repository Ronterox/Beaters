using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonFader : MonoBehaviour
{
    public bool faded = false;
    Image buttonImage;
    Text txt;
    Color buttonColor;
    Color textColor;
    bool startFade = false;
    float smooth = 0;
    bool initialized = false;
    private void Start() 
    {
        Initialize();
    }
    void Initialize()
    {
        startFade = false;
        faded = false;
        buttonImage = GetComponent<Image>();
        buttonColor = buttonImage.color;
        if(GetComponentInChildren<Text>())
        {
            txt = GetComponentInChildren<Text>();
            textColor = txt.color;
        }
        initialized = true;
    }
    private void Update() {
        if(startFade)
        {
            Fade(smooth);
            if(buttonColor.a > 0.9) faded = true;
        }
    }
    public void Fade(float rate){
        if(!initialized)Initialize();
        smooth = rate;
        startFade = true;
        buttonColor.a += rate;
        buttonImage.color = buttonColor;
        if(txt){
            textColor.a += rate;
            txt.color = textColor;
        }
    }
}
