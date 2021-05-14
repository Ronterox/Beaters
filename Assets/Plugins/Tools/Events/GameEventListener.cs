using UnityEngine;
using UnityEngine.Events;

namespace Plugins.Tools.Events
{
    public class GameEventListener : MonoBehaviour
    {
        [SerializeField] private GameEvent m_GameEvent;
        [SerializeField] private UnityEvent m_UnityEvent;

        private void Awake() => m_GameEvent.Register(this);

        private void OnDestroy() => m_GameEvent.Unregister(this);

        public void RaiseEvent() => m_UnityEvent?.Invoke();
    }
}
