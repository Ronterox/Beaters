using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Gacha{
public class MoveScaleBox : MonoBehaviour
{
    public Vector3 position, positionReward, scale, scaleReward;
    public int durationMove, durationScale;
    public GameObject reward, quad;
    

    void Start(){
        transform.DOMove(position, durationMove);
        transform.DOScale(scale, durationScale);
    }

    void Update(){
        if(transform.position == position){
            if(Input.GetMouseButtonDown(0)){
                quad.SetActive(true);
                ChildrenLoop();
            }
        }
    }

    void ChildrenLoop()
     {
         int children = reward.transform.childCount;
         for (int i = 0; i < children; ++i){
            reward.transform.GetChild(i).DOMove(positionReward, durationMove);
            reward.transform.GetChild(i).DOScale(scaleReward, durationMove);
            TimelineManager.StartTimeline();
         }
            
     }

    }
}
