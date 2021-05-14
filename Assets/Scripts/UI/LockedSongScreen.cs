using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LockedSongScreen : MonoBehaviour
{
    public Image itemSprite1, itemSprite2;
    public TMP_Text itemText1, itemText2;

    public void SetValues(Sprite sprite1, Sprite sprite2, string text1, string text2)
    {
        itemSprite1.sprite = sprite1;
        itemSprite2.sprite = sprite2;
        itemText1.text = text1;
        itemText2.text = text2;
    }
}
