using Plugins.Tools;
using UnityEngine;
using SoundManager = Plugins.Audio.SoundManager;

namespace Core
{
    [System.Serializable]
    public struct Instrument
    {
        public AudioClip c, d, e, f, g, a, b;
    }

    public class MapScroller : Singleton<MapScroller>
    {
        public float bpm;
        private bool m_IsStarted;
        [Space]
        public AudioClip mapSong;
        public Instrument instrument;

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

            SoundManager soundManager = SoundManager.Instance;
            soundManager.PlayBackgroundMusic(mapSong);
            soundManager.ResumeBackgroundMusic();
        }

        public void ResumeMap()
        {
            m_IsStarted = true;
            SoundManager.Instance.UnPauseBackgroundMusic();
        }

        public void StopMap()
        {
            m_IsStarted = false;
            SoundManager.Instance.PauseBackgroundMusic();
        }

        private void Update()
        {
            if (!m_IsStarted) return;
            transform.position -= new Vector3(0f, bpm * Time.deltaTime, 0f);
        }
    }
}
