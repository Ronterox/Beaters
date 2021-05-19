using DG.Tweening;
using Plugins.DOTween.Modules;
using UnityEngine;

namespace UI
{
    public class ButtonFader : MonoBehaviour
    {
        public CanvasGroup canvasGroup;
        public bool faded;

        private void OnEnable() => faded = false;
        
        public void Fade(float duration) => canvasGroup.DOFade(1f, duration).OnComplete(() => faded = true);
    }
}
