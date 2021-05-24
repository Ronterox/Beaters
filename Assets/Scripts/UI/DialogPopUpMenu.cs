using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DialogPopUpMenu : MonoBehaviour
{
    void Start()
    {

        if (!PlayerPrefs.HasKey("isFirstTime") || PlayerPrefs.GetInt("isFirstTime") != 1)
        {
            PlayerPrefs.SetInt("isFirstTime", 1);
            PlayerPrefs.Save();
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
