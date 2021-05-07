using UnityEngine;
using UnityEngine.EventSystems;

namespace SimpleFileBrowser
{
    public class CheckForEventSystem : MonoBehaviour
    {
        private void Awake()
        {
            if(EventSystem.current != GetComponent<EventSystem>()) Destroy(gameObject);
        }
    }
}
