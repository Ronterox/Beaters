using DG.Tweening;
using UnityEngine;

namespace Utilities
{
    public class PopUp : MonoBehaviour
    {
        public CanvasGroup canvasGroup;
        public float duration;

        private Tween m_ShowAnimation, m_HideAnimation;

        private void Start()
        {
            m_ShowAnimation = canvasGroup.DOFade(1f, duration).OnPlay(() => m_HideAnimation.Pause()).SetAutoKill(false);
            m_HideAnimation = canvasGroup.DOFade(0f, duration).OnPlay(() => m_ShowAnimation.Pause()).OnComplete(() => gameObject.SetActive(false)).SetAutoKill(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            canvasGroup.alpha = 0f;
            
            m_ShowAnimation.Restart();
            m_ShowAnimation.Play();
        }

        public void Hide()
        {
            m_HideAnimation.Restart();
            m_HideAnimation.Play();
        }
    }
}
