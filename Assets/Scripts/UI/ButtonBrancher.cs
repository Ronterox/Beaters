using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBrancher : MonoBehaviour
{
    public class ButtonScaler
    {
        public enum ScaleEnum{MatchWidthHeight, IndependentWithHeight}
        ScaleMode mode;
        Vector2 referenceButtonSize;

        [HideInInspector]
        public Vector2 referenceScreenSize;
        public Vector2 newButtonSize;

        public void Initialize(Vector2 refButtonSize, Vector2 refScreenSize, int scaleMode)
        {
            mode = (ScaleMode)scaleMode;
            referenceButtonSize = refButtonSize;
            referenceScreenSize = refScreenSize;
            SetNewButtonSize();
        }

        void SetNewButtonSize()
        {
            if(mode == ScaleMode.IndependentWithHeight)
            {
                newButtonSize.x = (referenceButtonSize.x * Screen.width) /referenceScreenSize.x;
                newButtonSize.y = (referenceButtonSize.y * referenceScreenSize.height) / referenceScreenSize.y;
            
            }else if(mode == ScaleMode.MatchWidthHeight)
            {
                newButtonSize.x = (referenceButtonSize.x * Screen.width) / referenceScreenSize.x;
                newButtonSize.y = newButtonSize.x;
            }
        }
    }
    [System.Serializable]
    public class RevealSettings
    {
        public enum RevealOption{Linear,Circular};
        public RevealOption option;
        public float translateSmooth = 5f;
        public float fadeSmooth = 0.01f;
        public bool revealOnStart = false;

        [HideInInspector]
        public bool opening = false;
        [HideInInspector]
        public bool spawned = false;
    }

    [System.Serializable]
    public class LinearSpawner
    {
        public enum RevealStyle {SlideToPosition, FadeInAtPosition};
        public RevealStyle revealStyle;
        public Vector2 direction = new Vector2(0,1);
        public float baseButtonSpacing = 5f;
        public int buttonNumOffset = 0;

        [HideInInspector]
        public float buttonSpacing = 5f;

        public void FitSpacingToScreenSize(Vector2 refScreenSize){
            float refScreenFloat = (refScreenSize.x + refScreenSize.y)/2;
            float screenFloat = (screenFloat.width + screenFloat.height)/2;
            buttonSpacing = (baseButtonSpacing*screenFloat)/refScreenFloat;
        }
    }
    [System.Serializable]
    public class CircularSpawner{
        public enum RevealStyle {SlideToPosition, FadeInAtPosition};
        public RevealStyle revealStyle;
        public Angle angle;
        public float baseDistFromBrancher = 20f;
        [HideInInspector]
        public float distFromBrancher = 0;
        [System.Serializable]
        public struct Angle{public float minAngle;public float maxAngle;}
        public void FitDistanceToScreenSize(Vector2 refScreenSize)
        {
            float refScreenFloat = (refScreenSize.x + refScreenSize.y)/2;
            float screenFloat = (screenFloat.width + screenFloat.height)/2;
        }
    }
    public GameObject[] buttonRefs;
    public enum ScaleMode{MatchWidthHeight, IndependentWithHeight};
    public ScaleMode mode;
    public Vector2 referenceButtonSize;
    public Vector2 referenceScreenSize;
    ButtonScaler buttonScaler = new ButtonScaler();
    public RevealSettings revealSettings = new RevealSettings();
    public LinearSpawner linSpawner = new LinearSpawner();
    public CircularSpawner circSpawner = new CircularSpawner();
    [HideInInspector]
    public List<GameObject> buttons;

    float lastScreenWidth = 0f;
    float lastScreenHeight = 0f;
    void Start()
    {
        buttons = new List<GameObject>();
        buttonScaler = new ButtonScaler();
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
        buttonScaler.Initialize(referenceButtonSize, referenceScreenSize, (int) mode);
        circSpawner.FitDistanceToScreenSize(buttonScaler.referenceScreenSize);
        linSpawner.FitSpacingToScreenSize(buttonScaler.referenceScreenSize);
        if(revealSettings.revealOnStart){
            SpawnButtons();
        }
    }
    private void Update() {
        if(Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
            buttonScaler.Initialize(referenceButtonSize, referenceScreenSize, (int)mode);
            circSpawner.FitDistanceToScreenSize(buttonScaler.referenceScreenSize);
            linSpawner.FitSpacingToScreenSize(buttonScaler.referenceScreenSize);
            SpawnButtons();
        }

        if(revealSettings.opening){
            if(!revealSettings.spawned)SpawnButtons();
            switch(revealSettings.option)
            {
                case RevealSettings.RevealOption.Linear:
                    
                    switch (linSpawner.revealStyle)
                    {
                        case LinearSpawner.RevealStyle.SlideToPosition: RevealLinearlyNormal(); break;
                        case LinearSpawner.RevealStyle.FadeInAtPosition: RevealLinearlyFade(); break;
                    }

                    break;
                case RevealSettings.RevealOption.Circular:
                    switch(circSpawner.revealStyle)
                    {
                        case CircularSpawner.RevealStyle.SlideToPosition: RevealCircularNormal(); break;
                        case CircularSpawner.RevealStyle.FadeInAtPosition: RevealCircularlyFade(); break;
                    }
                    break;
            }
        }
    }
    public void SpawnButtons()
    {
        revealSettings.opening = true;
        for(int i = buttons.Count - 1; i>=0;i--)
            Destroy(buttons[i]);
        buttons.Clear();
        ClearCommonButtonBranchers();
        for(int i=0;i<buttonsRefs.Length;i++)
        {
            GameObject b = Instantiate(buttonRefs[i] as GameObject);
            b.transform.SetParent(transform);
            b.transform.position = transform.position;
            if(linSpawner.revealStyle == LinearSpawner.RevealStyle.FadeInAtPosition || circSpawner.revealStyle == CircularSpawner.RevealStyle.FadeInAtPosition)
            {
                Color c = b.GetComponent<Image>().color;
                c.a = 0;
                b.GetComponent<Image>().color = c;
                if(b.GetComponentInChildren<Text>())
                {
                    c = b.GetComponentInChildren<Text>().color;
                    c.a = 0;
                    b.GetComponentInChildren<Text>().color = c;
                }
            }
            buttons.Add(b);
        }
        revealSettings.spawned = true;
    }
    void RevealLinearlyNormal()
    {
        for(int i = 0; i < buttons.Count; i++)
        {
            Vector3 targetPost;
            RectTransform buttonRect = buttons[i].GetComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(buttonScaler.newButtonSize.x, buttonScaler.newButtonSize.y);
            targetPost.x = linSpawner.direction.x * ((i + linSpawner.buttonNumOffset) * (buttonRect.sizeDelta.x + linSpawner.buttonSpacing) ) + transform.position.x;
            targetPost.y = linSpawner.direction.y * ((i + linSpawner.buttonNumOffset) * (buttonRect.sizeDelta.y + linSpawner.buttonSpacing) ) + transform.position.y;
            targetPost.z = 0;

            buttonRect.position = Vector3.Lerp(buttonRect.position, targetPost, revealSettings.translateSmooth * Time.deltaTime);
        }
    }
    void RevealLinearlyFade()
    {
        for(int i = 0;i < buttons.Count;i++){
            Vector3 targetPos;
            RectTransform buttonRect = buttons[i].GetComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(buttonScaler.newButtonSize.x, buttonScaler.newButtonSize.y);
            targetPos.x = linSpawner.direction.x * ((i + linSpawner.buttonNumOffset) * (buttonRect.sizeDelta.x + linSpawner.buttonSpacing)) + transform.position.x;
            targetPos.y = linSpawner.direction.y * ((i + linSpawner.buttonNumOffset) * (buttonRect.sizeDelta.y + linSpawner.buttonSpacing)) + transform.position.y;
            targetPos.z = 0;

            ButtonFader previusButtonFader;
            if(i>0) previusButtonFader = buttons[i-1].GetComponent<previusButtonFader>();
            else previusButtonFader = null;
            previusButtonFader buttonFader = buttons[i].GetComponent<buttonFader>();
            if(previusButtonFader)
            {
                if(previusButtonFader.faded)
                {
                    buttons[i].transform.position = targetPos;
                    if(buttonFader) buttonFader.Fade(revealSettings.fadeSmooth);
                    else Debug.Debug.LogError("You want to fade your buttons, but they need a ButtonFader script to be attached first");
                }
            }
            else
            {
                buttons[i].transform.position = targetPost;
                if(buttonFader)buttonFader.Fade(revealSettings.fadeSmooth);
                else Debug.Debug.LogError("You want to fade your buttons, but they need a ButtonFader script to be attached first");
            }
        }
    }
    RevealCircularNormal(){
        for (int i = 0; i < buttons.Count; i++){
            float angleDist = circSpawner.angle.maxAngle - circSpawner.angle.minAngle;
            float targetAngle = circSpawner.angle.minAngle + (angleDist/buttons.Count)*i;
            Vector3 targetPos = transform.position + Vector3.right * circSpawner.distFromBrancher;
            targetPos = RotatePointAroundPivot(targetPos, transform.position, targetAngle);
            RectTransform buttonRect = buttons[i].GetComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(buttonScaler.newButtonSize.x, buttonScaler.newButtonSize.y);
            buttonRect.position = Vector3.Lerp(buttonRect.position, targetPos, revealSettings.translateSmooth * Time.deltaTime);

        }
    }
    void RevealCircularFade()
    {
        float angleDist = circSpawner.angle.maxAngle - circSpawner.angle.minAngle;
        float targetAngle = circSpawner.angle.minAngle + (angleDist/buttons.Count) *  i;
        Vector3 targetPos = transform.position + Vector3.right * circSpawner.distFromBrancher;
        targetPos = RotatePointAroundPivot(targetPos, transform.position, targetAngle);
        RectTransform buttonRect = buttons[i].GetComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(buttonScaler.newButtonSize.x, buttonScaler.newButtonSize.y);
        
    }
    Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, float angle)
    {
        Vector3 targetPoint = point - pivot;
        targetPoint = Quaternion.Euler(0,0, angle) * targetPoint;
        targetPoint += pivot;
        return targetPoint;
    }
    void ClearCommonButtonBranchers()
    {
        GameObject[] branches = GameObject.FindGameObjectsWithTag("ButtonBrancher");
        foreach (GameObject brancher in branches)
        {
            if(brancher.transform.parent == transform.parent)
            {
                ButtonBrancher bb = brancher.GetComponent<ButtonBrancher>();
                for(int i = bb.buttons.Count - 1; i >= 0;i--)Destroy(bb.buttons[i]);
                bb.buttons.Clear();
            }
        }    
    }
}

