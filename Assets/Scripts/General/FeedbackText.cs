using DG.Tweening;
using Plugins.DOTween.Modules;
using TMPro;
using UnityEngine;

namespace General
{
    public class FeedbackText : MonoBehaviour
    {
        public TMP_Text text;
        public float animationDuration, aliveTime;
        [Space]
        public float upMovement;
        private Sequence m_ActivateAnimation;

        private void Start()
        {
            m_ActivateAnimation = DOTween.Sequence().SetAutoKill(false);
            m_ActivateAnimation.Join(text.DOFade(255, animationDuration).OnComplete(() =>
            {
                text.DOFade(0, animationDuration).OnComplete(() => gameObject.SetActive(false)).SetDelay(aliveTime);
            }));
            
            Vector3 position = transform.position;
            float movementDuration = animationDuration * .5f;
            
            m_ActivateAnimation.Join(text.transform.DOMoveY(position.y + upMovement, movementDuration));
            m_ActivateAnimation.Join(text.transform.DOMoveX(position.x + upMovement * Random.Range(-1, 2), movementDuration));
        }

        private void OnEnable()
        {
            m_ActivateAnimation.Restart();
            m_ActivateAnimation.Play();
        }
    }
}
