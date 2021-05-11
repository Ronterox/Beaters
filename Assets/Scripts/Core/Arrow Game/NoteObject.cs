using System;
using Managers;
using UnityEngine;

namespace Core.Arrow_Game
{
    public class NoteObject : MonoBehaviour
    {
        public ushort MakerId { get; set; }

        [Header("Config")]
        public Chord sound;
        public int comboLength;
        public bool isCombo;

        private SpriteRenderer m_SpriteRenderer;

        private ArrowButton m_ArrowButton;

        private bool m_WasPressed;

        private void Awake() => m_SpriteRenderer = GetComponent<SpriteRenderer>();

        public void SetCombo(int length)
        {
            isCombo = true;
            comboLength = length;
            m_SpriteRenderer.color = Color.yellow;
        }

#if !UNITY_IPHONE && !UNITY_ANDROID
        private bool m_OverButton;

        private void OnMouseDown()
        {
            if (m_OverButton) m_ArrowButton.PressButton();
        }
#endif

        private void OnButtonPressCallback()
        {
            m_WasPressed = true;

            GameplayManager.HitArrow(isCombo, comboLength);

            gameObject.SetActive(false);
            
            //TODO: Long note
            //SoundManager.Instance.PlayNonDiegeticSound(mapScroller.instrument.GetAudioClip(sound));
        }

        private void OnEnable() => m_WasPressed = false;

        private void OnDisable() => RemoveNoteCallbacks();

        private void RemoveNoteCallbacks()
        {
#if !UNITY_IPHONE && !UNITY_ANDROID
            m_OverButton = false;
#endif
            if (m_ArrowButton)
            {
                m_ArrowButton.isNoteAbove = false;
                m_ArrowButton.onButtonPress -= OnButtonPressCallback;
                m_ArrowButton = null;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            if (m_ArrowButton) RemoveNoteCallbacks();

            (m_ArrowButton = other.GetComponent<ArrowButton>()).onButtonPress += OnButtonPressCallback;
            m_ArrowButton.isNoteAbove = true;
#if !UNITY_IPHONE && !UNITY_ANDROID
            m_OverButton = true;
#endif
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player") || m_WasPressed) return;

            GameplayManager.MissArrow();
            gameObject.SetActive(false);
        }
    }
}
