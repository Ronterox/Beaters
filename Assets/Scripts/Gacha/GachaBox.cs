using UnityEngine;
using DG.Tweening;
using Plugins.Tools;
using UnityEngine.Playables;

namespace Gacha
{
    public class GachaBox : MonoBehaviour
    {
        public Transform boxTransform;
        public RandomLoot randomLoot;
        [Space]
        public PlayableDirector timeline;
        public CanvasGroup canvasGroup;

        [Header("Reward Config")]
        public GameObject reward;
        public Vector3 position, positionReward, scale, scaleReward;

        [Header("Animation Config")]
        public float fadeDuration;
        public float moveScaleDuration;

        private bool m_CanClick;

        private void Start()
        {
            boxTransform.DOMove(position, moveScaleDuration).OnComplete(() => m_CanClick = true);
            boxTransform.DOScale(scale, moveScaleDuration * 2);
        }

        private void Update()
        {
            if (!m_CanClick) return;

            if (Input.GetMouseButtonDown(0))
            {
                randomLoot.RandomItem();
                AnimateBox();
            }
        }

        private void AnimateBox()
        {

            reward.ForEachChild(child =>
            {
                Transform childTransform = child.transform;

                childTransform.transform.DOMove(positionReward, moveScaleDuration);
                childTransform.transform.DOScale(scaleReward, moveScaleDuration);
            });
            
            canvasGroup.alpha = 1;
            canvasGroup.DOFade(0f, fadeDuration).OnComplete(timeline.Play);
        }
    }
}
