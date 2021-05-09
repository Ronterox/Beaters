using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Plugins.GUI
{
    [AddComponentMenu("Penguins Mafia/GUI/Selectable Button")]
    public class SelectableButton : Button
    {
        public UnityEvent onSelect;
        public UnityEvent onDeselect;

        public void SetActions(UnityAction onClickAction, UnityAction onSelectAction = null, UnityAction onDeselectAction = null)
        {
            if(onClickAction != null) onClick.AddListener(onClickAction);
            if(onSelectAction != null) onSelect.AddListener(onSelectAction);
            if(onDeselectAction != null) onDeselect.AddListener(onDeselectAction);
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            onSelect?.Invoke();
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            onDeselect?.Invoke();
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            onSelect?.Invoke();
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            onDeselect?.Invoke();
        }
    }
}
