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

        public bool IsStarted { get; private set; }
        private float bps;

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
            IsStarted = true;
            transform.position.Set(0, 0,0);

            gameObject.SetActiveChildren(false);
            gameObject.SetActiveChildren();
            
            SoundManager.Instance.PlayBackgroundMusicNoFade(soundMap.audioClip, soundMap.startDelay);
            
            print("Started Map!");
        }

        public void ResumeMap()
        {
            IsStarted = true;
            SoundManager.Instance.UnPauseBackgroundMusic();

            print("Resumed Map!");
        }

        public void StopMap()
        {
            IsStarted = false;
            SoundManager.Instance.PauseBackgroundMusic();

            print("Stopped Map!");
        }

        private void Update()
        {
            if (!IsStarted) return;
            
            //TODO: Have song so we can use songDeltaTime
            transform.position -= new Vector3(0f, bps * Time.deltaTime, 0f);

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
