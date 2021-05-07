using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChangeBanner : MonoBehaviour
{
    public Sprite characterBannerSprite;
    public Sprite runeBannerSprite;
    public Image imageToChange;
    public TMP_Text textToChange;

    public void ChangeBannerUI(){
        if(imageToChange.sprite == characterBannerSprite){
            textToChange.text = "Rune Banner";
            imageToChange.sprite = runeBannerSprite;
        }else{
            textToChange.text = "Character Banner";
            imageToChange.sprite = characterBannerSprite;
        }
    }

}
