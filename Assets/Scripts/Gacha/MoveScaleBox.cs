using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Gacha{
public class MoveScaleBox : MonoBehaviour
{
    public Vector3 position, positionReward, scale, scaleReward;
    public float durationMove, durationScale;
    public float speedFade;
    public GameObject reward;
    public Canvas canvas;
    private CanvasGroup m_canvasGroup;


        void Start(){
        m_canvasGroup = canvas.GetComponent<CanvasGroup>();
        transform.DOMove(position, durationMove);
        transform.DOScale(scale, durationScale);
    }

    void Update(){
        if(transform.position == position){
            if(Input.GetMouseButtonDown(0)){
                ChildrenLoop();
            }
        }
    }

    void ChildrenLoop()
     {
         StartCoroutine(FadeOut());
         int children = reward.transform.childCount;
         for (int i = 0; i < children; ++i){
            reward.transform.GetChild(i).DOMove(positionReward, durationMove);
            reward.transform.GetChild(i).DOScale(scaleReward, durationMove);
            TimelineManager.StartTimeline();
         }
            
     }

      

        IEnumerator FadeOut()
        {
            m_canvasGroup.alpha = 1;
            while (m_canvasGroup.alpha <= 1)
            {
                m_canvasGroup.alpha -= Time.deltaTime * speedFade;
                yield return null;
            }
        }

    }
}
