using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class scaleApproachCircle : MonoBehaviour
{
    public Vector3 scale;
    public float durationScale;
    void Start()
    {
        transform.DOScale(scale, durationScale);
    }
}
