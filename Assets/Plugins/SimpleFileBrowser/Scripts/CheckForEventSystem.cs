using UnityEngine;
using UnityEngine.EventSystems;

namespace Plugins.SimpleFileBrowser.Scripts
{
    public class CheckForEventSystem : MonoBehaviour
    {
        private void Awake()
        {
            if(EventSystem.current != GetComponent<EventSystem>()) Destroy(gameObject);
        }
    }
}
