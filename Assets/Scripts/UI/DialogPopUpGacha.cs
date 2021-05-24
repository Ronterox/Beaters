using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DialogPopUpGacha : MonoBehaviour
{
    void Start()
    {
        if (!PlayerPrefs.HasKey("isFirstTimeGacha") || PlayerPrefs.GetInt("isFirstTimeGacha") != 1)
        {
            PlayerPrefs.SetInt("isFirstTimeGacha", 1);
            PlayerPrefs.Save();
        }
        else if((!PlayerPrefs.HasKey("isSecondTimeGacha") || PlayerPrefs.GetInt("isSecondTimeGacha") != 1))
        {
            PlayerPrefs.SetInt("isSecondTimeGacha", 1);
            PlayerPrefs.Save();
        }
        else Destroy(gameObject);
    }
}