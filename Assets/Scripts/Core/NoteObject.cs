using Plugins.Audio;
using UnityEngine;

namespace Core
{
    public class NoteObject : MonoBehaviour
    {
        private ArrowButton m_ArrowButton;

        public AudioClip noteSound;

        private void OnButtonPressCallback()
        {
            GameManager.Instance.HitArrow();
            RemoveNote();
            
            if (noteSound) SoundManager.Instance.PlayNonDiegeticSound(noteSound);
        }

        private void RemoveNote()
        {
            gameObject.SetActive(false);
            m_ArrowButton.isNoteAbove = false;
            m_ArrowButton.onButtonPress -= OnButtonPressCallback;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            print($"Entered Collider {other.name}");

            if (!other.CompareTag("Player")) return;
            
            (m_ArrowButton = other.GetComponent<ArrowButton>()).onButtonPress += OnButtonPressCallback;
            m_ArrowButton.isNoteAbove = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            print($"Exit Collider {other.name}");

            if (!other.CompareTag("Player")) return;

            GameManager.Instance.MissArrow();
            RemoveNote();
        }
    }
}
