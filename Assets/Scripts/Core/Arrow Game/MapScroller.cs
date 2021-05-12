using System;
using System.Collections;
using DG.Tweening;
using Managers;
using Plugins.Tools;
using UnityEngine;
using Utilities;
using SoundManager = Plugins.Audio.SoundManager;
using ReadOnly = Plugins.Properties.ReadOnlyAttribute;

namespace Core.Arrow_Game
{
    [Serializable]
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
        public bool generateCombos = true;

        [ReadOnly, SerializeField]
        private SoundMap m_SoundMap;

        public Difficulty difficulty;
        public Instrument instrument;
        [Space]
        public MakerNote[] makerNotes;
        public LayerMask notesLayer;

        [Header("Visual Feedback")]
        public Transform[] animateByBpm;
        [Space]
        public Vector3 targetScale, defaultScale;
        private float m_AnimationDuration;

        [Header("Song State Feedback")]
        public TimerUI startTimer;

        public bool IsStarted { get; private set; }
        private float m_Bps;

        private bool m_WaitingForBeat;
        private WaitForSeconds m_WaitForSeconds;

        private AudioClip m_CurrentSong;
        public int MapNotesQuantity => m_SoundMap.notes.Length;

        private void Start()
        {
            ResetPos();

            startTimer.onTimerStop += () =>
            {
                startTimer.timerText.text = "Go!";
                Action deactivate = () => startTimer.gameObject.SetActive(false);
                deactivate.DelayAction(1f);
            };
        }

        public void StartMap()
        {
            ResetPos();
            IsStarted = true;
            
            gameObject.ForEachChild(child => child.SetActiveChildren(false));
            gameObject.ForEachChild(child => child.SetActiveChildren());
            
            CameraManager.Instance.CanDoPanning = false;

            //Start Timer Setting
            startTimer.timerText.text = "Ready?";
            startTimer.gameObject.SetActive(true);

            Action activateTimer = () =>
            {
                startTimer.StartTimer();
                SoundManager.Instance.PlayBackgroundMusicNoFade(m_CurrentSong, 0, false);
            };
            
            activateTimer.DelayAction(1f);
            //__________________

            print("Started Map!");
        }

        public void ResetPos() => transform.position = Vector3.zero;

        public void ResumeMap()
        {
            IsStarted = true;
            SoundManager.Instance.UnPauseBackgroundMusic();

            CameraManager.Instance.CanDoPanning = false;

            print("Resumed Map!");
        }

        public void StopMap()
        {
            IsStarted = false;
            SoundManager.Instance.PauseBackgroundMusic();

            StopCoroutine(AnimateBeatCoroutine());
            m_WaitingForBeat = false;

            CameraManager.Instance.CanDoPanning = true;

            print("Stopped Map!");
        }

        private void Update()
        {
            if (!IsStarted) return;

            transform.position -= new Vector3(0f, m_Bps * SoundManager.songDeltaTime, 0f);

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

        public void SetSoundMap(SoundMap soundMap, bool generateMap = false)
        {
            m_SoundMap = soundMap;

            m_CurrentSong = m_SoundMap.audioClip;

            m_Bps = m_SoundMap.bpm / 60 * (float)difficulty;

            float songLength = m_SoundMap.audioClip.length;

            const float numberOfCellsPerSongSecond = 4f, dimensionDifference = 8f;

            CameraManager.boundsY2d.maximum = songLength * numberOfCellsPerSongSecond;
            CameraManager.boundsY3d.maximum = songLength * numberOfCellsPerSongSecond - dimensionDifference;

            float ms = 60000 / m_SoundMap.bpm;
            float secs = ms * 0.001f;

            m_WaitForSeconds = new WaitForSeconds(secs);
            m_AnimationDuration = secs * .5f;

            if (!generateMap) return;
            ResetPos();
            m_SoundMap?.GenerateNotes(makerNotes, transform, generateCombos);
        }
    }
}
