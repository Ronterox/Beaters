using Plugins.Tools;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Plugins.GUI
{
    public class MenuButton : MonoBehaviour, ISelectHandler, IPointerEnterHandler
    {

        public void OnSelect(BaseEventData eventData) => SoundManager.Instance.Play("Select");

        public void OnPointerEnter(PointerEventData eventData)
        {
            SoundManager.Instance.Play("Select");
            
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }
}
