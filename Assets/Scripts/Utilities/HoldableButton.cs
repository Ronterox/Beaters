using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Utilities
{
    public class HoldableButton : Button
    {
        public delegate void ButtonDownEvent();

        public event ButtonDownEvent onButtonDown;

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            onButtonDown?.Invoke();
        }
    }
}
