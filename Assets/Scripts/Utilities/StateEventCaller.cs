using UnityEngine;
using UnityEngine.Events;

namespace Utilities
{
    public class StateEventCaller : MonoBehaviour
    {
        public UnityEvent onAwake, onStart, onEnable, onDisable, onDestroy;

        private void Awake() => onAwake?.Invoke();

        private void Start() => onStart?.Invoke();

        private void OnEnable() => onEnable?.Invoke();

        private void OnDisable() => onDisable?.Invoke();

        private void OnDestroy() => onDestroy?.Invoke();
    }
}
