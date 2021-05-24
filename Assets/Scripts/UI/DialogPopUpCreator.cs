using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DialogPopUpCreator : MonoBehaviour
{
    void Start()
    {
        if (!PlayerPrefs.HasKey("isFirstTimeCreator") || PlayerPrefs.GetInt("isFirstTimeCreator") != 1)
        {
            PlayerPrefs.SetInt("isFirstTimeCreator", 1);
            PlayerPrefs.Save();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}