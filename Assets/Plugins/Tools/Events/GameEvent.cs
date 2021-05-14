using System.Collections.Generic;
using UnityEngine;

namespace Plugins.Tools.Events
{
    [CreateAssetMenu(fileName = "New Game Event", menuName = "Penguins Mafia/Game Event")]
    public class GameEvent : ScriptableObject
    {
        private readonly HashSet<GameEventListener> m_Listeners = new HashSet<GameEventListener>();
        
        public void Invoke()
        {
            foreach (GameEventListener gameEventListener in m_Listeners) gameEventListener.RaiseEvent();
        }

        public void Register(GameEventListener listener) => m_Listeners.Add(listener);

        public void Unregister(GameEventListener listener) => m_Listeners.Remove(listener);
    }
}
