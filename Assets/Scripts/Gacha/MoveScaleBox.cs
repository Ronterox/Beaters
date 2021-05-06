using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Gacha{
public class MoveScaleBox : MonoBehaviour
{
    public Vector3 position, scale;
    public int durationMove, durationScale;

    void Start(){
        transform.DOMove(position, durationMove);
        transform.DOScale(scale, durationScale);
    }

    void Update(){
        if(transform.position == position){
            if(Input.GetMouseButtonDown(0)){
                gameObject.SetActive(false);
            }
        }
    }

    }
}
