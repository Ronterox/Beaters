using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementButtons : MonoBehaviour
{
    public GameObject achievements, trophies, achievementsLayout, trophiesLayout;
    public void ChangeLayout()
    {
        if (achievementsLayout.activeSelf)
        {
            trophiesLayout.SetActive(true);
            achievementsLayout.SetActive(false);
        }
        else
        {
            achievementsLayout.SetActive(true);
            trophiesLayout.SetActive(false);
        }
    }
}
