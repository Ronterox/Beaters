using UnityEngine;
using DG.Tweening;
using Managers;
using Plugins.Properties;
using Plugins.Tools;
using UnityEngine.Playables;

namespace Gacha
{
    public class GachaBox : MonoBehaviour
    {
        [Scene]
        public string gachaMenuScene;
        [Space]
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

        private bool m_CanClick, m_SawPrize;

        private void Start()
        {
            boxTransform.DOMove(position, moveScaleDuration).OnComplete(() => m_CanClick = true);
            boxTransform.DOScale(scale, moveScaleDuration * 2);

            timeline.stopped += x => m_CanClick = m_SawPrize = true;
        }

        private void Update()
        {
            if (!m_CanClick) return;

            if (Input.GetMouseButtonDown(0))
            {
                if (m_SawPrize)
                {
                    m_CanClick = false;
                    LevelLoadManager.LoadSceneWithTransition(gachaMenuScene, .5f);
                }
                else
                {
                    randomLoot.RandomItem();
                    AnimateBox();
                }
            }
        }

        private void AnimateBox()
        {
            m_CanClick = false;
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
