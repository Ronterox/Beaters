using Plugins.Audio;
using UnityEngine;

namespace Core
{
    public class FallingArrow : MonoBehaviour
    {
        private ArrowButton m_ArrowButton;

        public AudioClip noteSound;

        private void OnButtonPressCallback()
        {
            gameObject.SetActive(false);
            GameManager.Instance.HitArrow();

            if (noteSound) SoundManager.Instance.PlayNonDiegeticSound(noteSound);
            
            m_ArrowButton.onButtonPress -= OnButtonPressCallback;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                (m_ArrowButton = other.GetComponent<ArrowButton>()).onButtonPress += OnButtonPressCallback;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            m_ArrowButton.onButtonPress -= OnButtonPressCallback;
            GameManager.Instance.MissArrow();
        }
    }
}
