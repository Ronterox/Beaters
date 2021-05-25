using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Utilities
{
    public class HoldableButton : Button
    {
        public delegate void ButtonEvent();

        public event ButtonEvent onButtonDown, onButtonUp;

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            onButtonDown?.Invoke();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            onButtonUp?.Invoke();
        }
    }
}
