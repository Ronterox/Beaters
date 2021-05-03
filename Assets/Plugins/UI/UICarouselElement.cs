using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Plugins.UI
{
    public abstract class UICarouselElement : Button
    {
        [System.Serializable]
        public struct ElementAnim
        {
            public float scale;
            public float duration;
            //public Ease easeType;
        }

        public int ID;
        public ElementAnim selectAnim;
        public ElementAnim deselectAnim;

        public delegate void UICarouselElementEvent(UICarouselElement element);

        public event UICarouselElementEvent onSelectElement;
        public event UICarouselElementEvent onDeselectElement;

        public RectTransform RectTransform { get; private set; }

        protected override void OnEnable()
        {
            base.OnEnable();
            RectTransform = transform as RectTransform;
            //PlayDeselectAnim();
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            onSelectElement?.Invoke(this);
            PlaySelectAnim();
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            onDeselectElement?.Invoke(this);
            PlayDeselectAnim();
        }

        public virtual void PlaySelectAnim() { }

        //transform.DOScale(selectAnim.scale, selectAnim.duration).SetEase(selectAnim.easeType).OnUpdate(() =>
            //{
               // LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent as RectTransform);
           // });

        public virtual void PlayDeselectAnim()
        {
            //transform.DOScale(deselectAnim.scale, deselectAnim.duration).SetEase(deselectAnim.easeType);
        }

        public abstract UICarouselElement Setup(params object[] parameters);
    }
}
