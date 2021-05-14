using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecordScreen : MonoBehaviour
{
    public Image characterImage;
    public TMP_Text scoreText, gradeText, comboText, accuracyText; 

    public void SetValues(Sprite sprite, int score, string grade, int combo, string accuracy)
    {
        characterImage.sprite = sprite;
        scoreText.text = score + "";
        gradeText.text = grade;
        comboText.text = combo + "";
        accuracyText.text = accuracy;
    }
}
