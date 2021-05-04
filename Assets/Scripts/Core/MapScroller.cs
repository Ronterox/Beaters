using System.Collections;
using DG.Tweening;
using Managers;
using Plugins.Tools;
using UnityEngine;
using Utilities;
using SoundManager = Plugins.Audio.SoundManager;

namespace Core
{
    [System.Serializable]
    public struct Instrument
    {
        public AudioClip c, d, e, f, g, a, b;

        public AudioClip GetAudioClip(Chord chord) =>
            chord switch
            {
                Chord.C => c,
                Chord.D => d,
                Chord.E => e,
                Chord.F => f,
                Chord.G => g,
                Chord.A => a,
                Chord.B => b,
                _ => c
            };
    }

    public enum Chord { C, D, E, F, G, A, B }

    public class MapScroller : MonoBehaviour
    {
        [Plugins.Properties.ReadOnly]
        public SoundMap soundMap;
        public Difficulty difficulty;
        public Instrument instrument;

        [Header("Visual Feedback")]
        public Transform[] animateByBpm;

        [Space]
        public Vector3 targetScale, defaultScale;
        private float m_AnimationDuration;

        private float bps;
        private bool m_IsStarted;

        private bool m_WaitingForBeat;
        private WaitForSeconds m_WaitForSeconds;

        private void Awake()
        {
            bps = soundMap.bpm / 60 * (float)difficulty;

            float ms = 60000 / soundMap.bpm;
            float secs = ms * 0.001f;

            m_WaitForSeconds = new WaitForSeconds(secs);
            m_AnimationDuration = secs * .5f;
        }

        public void StartMap()
        {
            m_IsStarted = true;
            transform.position.Set(0, 0,0);

            gameObject.SetActiveChildren(false);
            gameObject.SetActiveChildren();
            
            SoundManager.Instance.PlayBackgroundMusicNoFade(soundMap.audioClip);
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
            transform.position -= new Vector3(0f, bps * SoundManager.songDeltaTime, 0f);

            AnimateBeat();
        }

        private void AnimateBeat()
        {
            if (m_WaitingForBeat) return;
            StartCoroutine(AnimateBeatCoroutine());
        }

        private IEnumerator AnimateBeatCoroutine()
        {
            m_WaitingForBeat = true;

            animateByBpm.ForEach(t => t.DOScale(targetScale, m_AnimationDuration).OnComplete(() => t.DOScale(defaultScale, m_AnimationDuration)));

            yield return m_WaitForSeconds;
            m_WaitingForBeat = false;
        }
    }
}
