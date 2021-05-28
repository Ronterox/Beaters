using UnityEngine;
using DG.Tweening;
using Managers;
using Plugins.Audio;
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
        public Transform rewardTransform;
        public SpriteRenderer[] sprites;
        public Vector3 position, positionReward, scale, scaleReward;

        [Header("Animation Config")]
        public float fadeDuration;
        public float moveScaleDuration;

        [Header("Sfx")]
        public AudioClip gachaThrowSoundEffect;
        
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
                GameManager.PutValue(BingoManager.OPEN_BINGO_GACHA);
                LevelLoadManager.LoadSceneWithTransition(gachaMenuScene, .5f);
            }
            else AnimateBox();
        }

        private void AnimateBox()
        {
            m_CanClick = false;

            SetRewardObject();

            rewardTransform.DOMove(positionReward, moveScaleDuration);
            rewardTransform.DOScale(scaleReward, moveScaleDuration);

            canvasGroup.alpha = 1;
            canvasGroup.DOFade(0f, fadeDuration).OnComplete(timeline.Play);
            
            SoundManager.Instance.PlayNonDiegeticSound(gachaThrowSoundEffect);
        }

        private void SetRewardObject()
        {
            ScriptableObject[] prizes = GameManager.GetPrizes();

            for (var i = 0; i < prizes.Length; i++)
            {
                SpriteRenderer currentRenderer = sprites[i];
                currentRenderer.gameObject.SetActive(true);
                currentRenderer.sprite = GetPrizeVisualization(prizes[i]);
            }
        }

        private Sprite GetPrizeVisualization(ScriptableObject scriptableObject) =>
            scriptableObject switch
            {
                ScriptableCharacter character => character.sprites[0],
                ScriptableItem item => item.itemSprite,
                ScriptableRune rune => rune.runeSprite,
                _ => null
            };
    }
}
