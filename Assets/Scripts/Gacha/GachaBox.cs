using UnityEngine;
using DG.Tweening;
using Managers;
using Plugins.Properties;
using ScriptableObjects;
using UnityEngine.Playables;

namespace Gacha
{
    public class GachaBox : MonoBehaviour
    {
        [Scene]
        public string gachaMenuScene;
        [Space]
        public Transform boxTransform;
        [Space]
        public PlayableDirector timeline;
        public CanvasGroup canvasGroup;

        [Header("Reward Config")]
        public SpriteRenderer rewardSpriteRenderer;
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
            if (!m_CanClick || !Input.GetMouseButtonDown(0)) return;

            if (m_SawPrize)
            {
                m_CanClick = false;
                LevelLoadManager.LoadSceneWithTransition(gachaMenuScene, .5f);
            }
            else AnimateBox();
        }

        private void AnimateBox()
        {
            m_CanClick = false;

            SetRewardObject();

            Transform reward = rewardSpriteRenderer.transform;

            reward.DOMove(positionReward, moveScaleDuration);
            reward.DOScale(scaleReward, moveScaleDuration);

            canvasGroup.alpha = 1;
            canvasGroup.DOFade(0f, fadeDuration).OnComplete(timeline.Play);
        }

        private void SetRewardObject() => rewardSpriteRenderer.sprite = GameManager.GetPrize() switch
        {
            ScriptableCharacter character => character.sprites[0],
            ScriptableItem item => item.itemSprite,
            ScriptableRune rune => rune.runeSprite,
            _ => rewardSpriteRenderer.sprite
        };
    }
}
