using Managers;
using UnityEngine;

namespace Core
{
    public class NoteObject : MonoBehaviour
    {
        public ushort MakerId { get; set; }
        
        [Header("Config")]
        public Chord sound;

        private ArrowButton m_ArrowButton;

        private bool m_WasPressed, m_OverButton;

        private void OnMouseDown()
        {
            if (m_OverButton) m_ArrowButton.PressButton();
        }

        private void OnButtonPressCallback()
        {
            m_WasPressed = true;

            GameManager.Instance.HitArrow();

            gameObject.SetActive(false);

            //TODO: Long note
            //SoundManager.Instance.PlayNonDiegeticSound(mapScroller.instrument.GetAudioClip(sound));
        }

        private void OnDisable() => RemoveNoteCallbacks();

        private void RemoveNoteCallbacks()
        {
            m_OverButton = false;
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
            m_OverButton = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player") && !m_WasPressed) return;

            GameManager.Instance.MissArrow();
            gameObject.SetActive(false);
        }
    }
}
