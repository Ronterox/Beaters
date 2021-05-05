using Core;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Utilities
{
    public class HoldableButton : Button
    {
        public ArrowButton.ButtonEvent onButtonDown;

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            onButtonDown?.Invoke();
        }
    }
}
