using DG.Tweening;
using Managers;
using UnityEngine;

namespace Core
{
    public class ArrowButton : MonoBehaviour
    {
        [Header("Animations")]
        public float animationDuration;
        public Vector3 targetScale;
        private Vector3 m_DefaultScale;

        [Plugins.Properties.ReadOnly]
        public bool isNoteAbove;

        public delegate void ButtonEvent();

        public event ButtonEvent onButtonPress, onButtonRelease;

        private void Awake() => m_DefaultScale = transform.localScale;

        private void OnEnable() => onButtonPress += CheckButton;

        private void OnDisable() => onButtonPress -= CheckButton;

        public void PressButton() => onButtonPress?.Invoke();

        private void CheckButton()
        {
            if (!isNoteAbove) GameManager.Instance.MissArrow();
            //Arrow animation with tween
            transform.DOScale(targetScale, animationDuration).OnComplete(() => transform.DOScale(m_DefaultScale, animationDuration));
        }

        private void OnMouseDown() => onButtonPress?.Invoke();

        private void OnMouseUp() => onButtonRelease?.Invoke();
    }
}
