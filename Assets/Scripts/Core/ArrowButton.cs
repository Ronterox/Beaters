using UnityEngine;

namespace Core
{
    public class ArrowButton : MonoBehaviour
    {
        public delegate void ButtonEvent();

        public event ButtonEvent onButtonPress, onButtonRelease;
        
        private void OnMouseDown() => onButtonPress?.Invoke();

        private void OnMouseUp() => onButtonRelease?.Invoke();
    }
}
