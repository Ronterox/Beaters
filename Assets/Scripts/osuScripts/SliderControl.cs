using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Plugins.Tools;

public class SliderControl : MonoBehaviour
{
    public List<Transform> waypoints;
    public float durationMove;

    void Start()
    {        
        StartCoroutine(Movement());
    }

     private IEnumerator Movement(){
      foreach (Transform t in waypoints)
      {
          transform.DOMove(t.position, durationMove);
          yield return new WaitUntil(() => t.position.Approximates(transform.position, 1.3f));
      }
 }
}
