using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DialogPopUpSong : MonoBehaviour
{
    void Start()
    {
        if (!PlayerPrefs.HasKey("isFirstTimeSong") || PlayerPrefs.GetInt("isFirstTimeSong") != 1)
        {
            PlayerPrefs.SetInt("isFirstTimeSong", 1);
            PlayerPrefs.Save();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}