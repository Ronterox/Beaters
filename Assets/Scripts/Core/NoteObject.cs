using System;
using Plugins.Audio;
using UnityEngine;

namespace Core
{
    public class NoteObject : MonoBehaviour
    {
        private ArrowButton m_ArrowButton;

        public AudioClip noteSound;

        private bool m_WasPressed, m_OverButton;

        private void OnMouseDown()
        {
            if (m_OverButton) m_ArrowButton.PressButton();
        }

        private void OnButtonPressCallback()
        {
            m_WasPressed = true;

            GameManager.Instance.HitArrow();
            RemoveNote();

            if (noteSound) SoundManager.Instance.PlayNonDiegeticSound(noteSound);
        }

        private void RemoveNote()
        {
            m_OverButton = false;
            gameObject.SetActive(false);
            m_ArrowButton.isNoteAbove = false;
            m_ArrowButton.onButtonPress -= OnButtonPressCallback;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            (m_ArrowButton = other.GetComponent<ArrowButton>()).onButtonPress += OnButtonPressCallback;
            m_ArrowButton.isNoteAbove = true;
            m_OverButton = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player") && !m_WasPressed) return;

            GameManager.Instance.MissArrow();
            RemoveNote();
        }
    }
}
