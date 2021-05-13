using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GreatBadMove : MonoBehaviour
{
    public Vector3 endPosition;
    public float duration;

    void Start()
    {
        StartCoroutine(Destruction());
    }

     private IEnumerator Destruction(){
          transform.DOMove(endPosition, duration);
          yield return new WaitForSeconds(duration-0.5f);
          Destroy(gameObject);
 }
}
