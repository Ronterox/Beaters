using Managers;
using Plugins.Properties;
using UnityEngine;

namespace Core.Arrow_Game
{
    public class NoteObject : MonoBehaviour
    {
        public Camera mainCamera;
        public ushort MakerId { get; set; }

        [Header("Config")]
        public Chord sound;
        public SpriteRenderer spriteRenderer;

        [TagSelector]
        public string buttonTag;

        public int comboLength;
        public bool isCombo;

        private ArrowButton m_ArrowButton;

        private bool m_WasPressed;

        private void Awake()
        {
            if (!mainCamera) mainCamera = Camera.main;
        }

        public void SetCombo(int length)
        {
            isCombo = true;
            comboLength = length;
            spriteRenderer.color = Color.yellow;
        }

#if !UNITY_IPHONE && !UNITY_ANDROID
        private bool m_OverButton;

        private void OnMouseDown()
        {
            if (m_OverButton) m_ArrowButton.PressButton();
        }
#endif

        private void OnButtonPressCallback(float buttonHeight)
        {
            m_WasPressed = true;

            HitType hitType = GetHitType(buttonHeight);

            GameplayManager.Instance.HitArrow(hitType, mainCamera.WorldToScreenPoint(transform.position), spriteRenderer.color, isCombo, comboLength);

            gameObject.SetActive(false);

            //TODO: Long note
            //SoundManager.Instance.PlayNonDiegeticSound(mapScroller.instrument.GetAudioClip(sound));
        }

        private HitType GetHitType(float buttonHeight)
        {
            float noteHeight = transform.position.y;
            float difference = Mathf.Abs(noteHeight - buttonHeight);

            if (difference < .5f) return HitType.Perfect;
            if (difference < 1f) return HitType.Good;
            return noteHeight > buttonHeight ? HitType.TooSoon : HitType.TooSlow;
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
            if (!other.CompareTag(buttonTag)) return;

            if (m_ArrowButton) RemoveNoteCallbacks();

            (m_ArrowButton = other.GetComponent<ArrowButton>()).onButtonPress += OnButtonPressCallback;
            m_ArrowButton.isNoteAbove = true;
#if !UNITY_IPHONE && !UNITY_ANDROID
            m_OverButton = true;
#endif
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag(buttonTag) || m_WasPressed) return;

            GameplayManager.Instance.MissArrow();
            gameObject.SetActive(false);
        }
    }
}
