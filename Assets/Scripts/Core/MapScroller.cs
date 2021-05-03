using Plugins.Tools;
using UnityEngine;

namespace Core
{
    public class MapScroller : Singleton<MapScroller>
    {
        public float bpm;
        private bool m_IsStarted;

        protected override void Awake()
        {
            base.Awake();
            bpm /= 60;
        }

        public void StartMap()
        {
            m_IsStarted = true;
            transform.position = Vector3.zero;
            
            gameObject.SetActiveChildren();
        }

        public void ResumeMap() => m_IsStarted = true;

        public void StopMap() => m_IsStarted = false;

        private void Update()
        {
            if (!m_IsStarted) return;
            transform.position -= new Vector3(0f, bpm * Time.deltaTime, 0f);
        }
    }
}
