using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Plugins.UI
{
    public abstract class UICarousel : ScrollRect
    {
        public GameObject elementPrefab;
        public RectTransform scrollMask;

        [Header("Animation")]
        public float scrollSpeed;
        public Ease scrollEase;

        public delegate void UICarouselEvent(UICarouselElement element);

        public event UICarouselEvent onSelectElement;
        public event UICarouselEvent onDeselectElement;

        protected List<UICarouselElement> m_Elements;
        protected RectTransform m_RectTransform;

        public UICarouselElement SelectedElement { get; protected set; }
        public bool IsEmpty => m_Elements == null || m_Elements.Count < 1;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_RectTransform = transform as RectTransform;

            m_Elements ??= new List<UICarouselElement>();
        }

        public virtual UICarouselElement CreateElement(int id, bool selected)
        {
            GameObject elementGO = Instantiate(elementPrefab, content);
            var element = elementGO.GetComponent<UICarouselElement>();

            element.ID = id;
            element.onSelectElement += OnElementSelect;
            element.onDeselectElement += OnElementDeselect;

            m_Elements.Add(element);

            if (selected)
            {
                element.PlaySelectAnim();
                SelectedElement = element;
            }
            else element.PlayDeselectAnim();

            return element;
        }

        public virtual void CreateNavigationLinks()
        {
            var prev = new Navigation
            {
                mode = Navigation.Mode.Explicit,
                selectOnRight = m_Elements[1].interactable ? m_Elements[1] : null
            };
            m_Elements[0].navigation = prev;

            for (var i = 1; i < m_Elements.Count - 1; i++)
            {
                var current = new Navigation
                {
                    mode = Navigation.Mode.Explicit,
                    selectOnLeft = m_Elements[i - 1].interactable ? m_Elements[i - 1] : null,
                    selectOnRight = m_Elements[i + 1].interactable ? m_Elements[i + 1] : null
                };
                m_Elements[i].navigation = current;
            }

            var next = new Navigation
            {
                mode = Navigation.Mode.Explicit,
                selectOnLeft = m_Elements[m_Elements.Count - 2].interactable ? m_Elements[m_Elements.Count - 2] : null,
            };
            m_Elements[m_Elements.Count - 1].navigation = next;
        }

        public void ScrollToSelectedElement()
        {
            if (SelectedElement != null) ScrollToElement(SelectedElement);
        }

        public void SelectElement(int index)
        {
            if (IsEmpty) return;

            UICarouselElement element = m_Elements[Mathf.Clamp(index, 0, m_Elements.Count - 1)];
            SelectElement(element);
        }

        public void SelectElement(UICarouselElement element)
        {
            if (!element) return;
            EventSystem.current.SetSelectedGameObject(element.gameObject);
            OnElementSelect(element);
        }

        public void DeselectCurrentElement()
        {
            if (!SelectedElement) return;
            OnElementDeselect(SelectedElement);
        }

        protected virtual void OnElementSelect(UICarouselElement element)
        {
            SelectedElement = element;
            ScrollToElement(element);
            onSelectElement?.Invoke(element);
        }

        protected virtual void OnElementDeselect(UICarouselElement element) => onDeselectElement?.Invoke(element);

        protected virtual void ScrollToElement(UICarouselElement element)
        {
            // Where the element is inside the scroll rect
            Vector3 elementCenterPos = GetWorldPointInWidget(m_RectTransform, GetWidgetWorldPoint(element.RectTransform));

            // Where the element should be inside the scroll rect
            Vector3 targetPos = GetWorldPointInWidget(m_RectTransform, GetWidgetWorldPoint(scrollMask));

            Vector3 difference = targetPos - elementCenterPos;

            if (!horizontal) difference.x = 0f;

            if (!vertical) difference.y = 0f;

            Vector2 normalizedDifference;
            
            Rect rect = content.rect, rect1 = m_RectTransform.rect;
            normalizedDifference.x = difference.x / (rect.size.x - rect1.size.x);
            normalizedDifference.y = difference.y / (rect.size.y - rect1.size.y);

            Vector2 newNormalizedPosition = normalizedPosition - normalizedDifference;

            if (movementType != MovementType.Unrestricted)
            {
                newNormalizedPosition.x = Mathf.Clamp01(newNormalizedPosition.x);
                newNormalizedPosition.y = Mathf.Clamp01(newNormalizedPosition.y);
            }

            DOTween.To(() => normalizedPosition, x => normalizedPosition = x, newNormalizedPosition, 1f / scrollSpeed).SetEase(scrollEase);
        }

        private Vector3 GetWidgetWorldPoint(RectTransform target)
        {
            Vector3 pivotOffset = Vector3.zero;
            pivotOffset.x = (0.5f - target.pivot.x) * target.rect.size.x;
            pivotOffset.y = (0.5f - target.pivot.y) * target.rect.size.y;

            Vector3 localPosition = target.localPosition + pivotOffset;
            return target.parent.TransformPoint(localPosition);
        }

        private Vector3 GetWorldPointInWidget(Transform target, Vector3 worldPoint) => target.InverseTransformPoint(worldPoint);
    }
}
