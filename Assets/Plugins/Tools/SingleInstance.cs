using UnityEngine;

namespace Plugins.Tools
{
    /// <summary>
    /// Singleton pattern.
    /// </summary>
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        protected static T m_Instance;

        /// <summary>
        /// Singleton design pattern
        /// </summary>
        /// <value>The Instance.</value>
        
        public static T Instance => !m_Instance? 
            m_Instance = FindObjectOfType<T>()? m_Instance : m_Instance = new GameObject().AddComponent<T>() : m_Instance;

        /// <summary>
        /// On awake, we initialize our Instance. Make sure to call base.Awake() in override if you need awake.
        /// </summary>
        protected virtual void Awake()
        {
            if (!Application.isPlaying) return;

            if (!m_Instance) m_Instance = this as T;
            else if (m_Instance != this) { Debug.Log($"There is another {m_Instance.name} script on the scene!!!"); }
        }
    }

    public class PersistentSingleton<T> : MonoBehaviour where T : Component
    {
        protected static T m_Instance;

        /// <summary>
        /// Singleton design pattern
        /// </summary>
        /// <value>The Instance.</value>
        public static T Instance => !m_Instance? 
            m_Instance = FindObjectOfType<T>()? m_Instance : m_Instance = new GameObject().AddComponent<T>() : m_Instance;

        /// <summary>
        /// On awake, we check if there's already a copy of the object in the scene. If there's one, we destroy it.
        /// </summary>
        protected virtual void Awake()
        {
            if (!m_Instance)
            {
                //If I am the first Instance, make me the Singleton
                m_Instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                //If a Singleton already exists and you find
                //another reference in scene, destroy it!
                if (this == m_Instance) return;
                Debug.Log($"There was another {m_Instance.name} script on the scene, so it was destroyed!");
                Destroy(gameObject);
            }
        }
    }
}
