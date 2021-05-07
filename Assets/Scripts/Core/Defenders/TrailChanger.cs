using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailChanger : MonoBehaviour
{
    GameObject upTrail, downTrail, leftTrail, rightTrail;
    void Start()
    {
        upTrail.SetActive = true;
        downTrail.SetActive = true;
        leftTrail.SetActive = true;
        rightTrail.SetActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
