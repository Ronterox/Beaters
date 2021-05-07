using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Plugins.GUI
{
    public class MenuButton : Button
    {

       // public void OnSelect(BaseEventData eventData) => SoundManager.Instance.Play("Select");

        public override void OnPointerEnter(PointerEventData eventData)
        {
           // SoundManager.Instance.Play("Select");
            
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }
}
