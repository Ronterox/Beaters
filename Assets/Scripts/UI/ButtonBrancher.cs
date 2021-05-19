using Plugins.Tools;
using UnityEngine;

namespace UI
{
    public class ButtonBrancher : MonoBehaviour
    {
        public class ButtonScaler
        {
            private ScaleMode m_ScaleMode;
            private Vector2 m_ReferenceButtonSize;

            public Vector2 referenceScreenSize;
            public Vector2 newButtonSize;

            public void Initialize(Vector2 refButtonSize, Vector2 refScreenSize, int scaleMode)
            {
                m_ScaleMode = (ScaleMode)scaleMode;
                m_ReferenceButtonSize = refButtonSize;
                referenceScreenSize = refScreenSize;
                SetNewButtonSize();
            }

            private void SetNewButtonSize()
            {
                switch (m_ScaleMode)
                {
                    case ScaleMode.IndependentWithHeight:
                        newButtonSize.x = m_ReferenceButtonSize.x * Screen.width / referenceScreenSize.x;
                        newButtonSize.y = m_ReferenceButtonSize.y * Screen.height / referenceScreenSize.y;
                        break;
                    case ScaleMode.MatchWidthHeight:
                        newButtonSize.x = m_ReferenceButtonSize.x * Screen.width / referenceScreenSize.x;
                        newButtonSize.y = newButtonSize.x;
                        break;
                }
            }
        }
        [System.Serializable]
        public class RevealSettings
        {
            public float translateSmooth = 5f;
            public float fadeSmooth = 0.01f;
            public bool revealOnStart;

            [HideInInspector]
            public bool opening;
            [HideInInspector]
            public bool spawned;
        }

        [System.Serializable]
        public class LinearSpawner
        {
            public enum RevealStyle { SlideToPosition, FadeInAtPosition }

            public RevealStyle revealStyle;
            public Vector2 direction = new Vector2(0, 1);
            public float baseButtonSpacing = 5f;
            public int buttonNumOffset;

            [HideInInspector]
            public float buttonSpacing = 5f;

            public void FitSpacingToScreenSize(Vector2 refScreenSize)
            {
                float refScreenFloat = (refScreenSize.x + refScreenSize.y) * .5f;
                float screenFloat = (Screen.width + Screen.height) * .5f;
                buttonSpacing = baseButtonSpacing * screenFloat / refScreenFloat;
            }
        }
        public ButtonFader[] buttonRefs;

        public enum ScaleMode { MatchWidthHeight, IndependentWithHeight }

        public ScaleMode mode;

        public Vector2 referenceButtonSize, referenceScreenSize;

        private ButtonScaler m_ButtonScaler = new ButtonScaler();

        public RevealSettings revealSettings = new RevealSettings();

        public LinearSpawner linSpawner = new LinearSpawner();

        private int lastScreenWidth, lastScreenHeight;
        private void Start()
        {
            m_ButtonScaler = new ButtonScaler();

            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;

            m_ButtonScaler.Initialize(referenceButtonSize, referenceScreenSize, (int)mode);

            linSpawner.FitSpacingToScreenSize(m_ButtonScaler.referenceScreenSize);

            if (revealSettings.revealOnStart) SpawnButtons();
        }
        private void Update()
        {
            if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
            {
                lastScreenWidth = Screen.width;
                lastScreenHeight = Screen.height;

                m_ButtonScaler.Initialize(referenceButtonSize, referenceScreenSize, (int)mode);

                linSpawner.FitSpacingToScreenSize(m_ButtonScaler.referenceScreenSize);

                SpawnButtons();
            }

            if (!revealSettings.opening) return;

            if (!revealSettings.spawned) SpawnButtons();

            switch (linSpawner.revealStyle)
            {
                case LinearSpawner.RevealStyle.SlideToPosition:
                    RevealLinearlyNormal();
                    break;
                case LinearSpawner.RevealStyle.FadeInAtPosition:
                    RevealLinearlyFade();
                    break;
            }
        }
        
        public void SpawnButtons()
        {
            revealSettings.opening = true;

            SetButtonRefsEnable(false);

            Transform tform = transform;
            buttonRefs.ForEach(buttonRef => buttonRef.transform.position = tform.position);

            SetButtonRefsEnable(true);

            revealSettings.spawned = true;
        }
        
        private void RevealLinearlyNormal()
        {
            for (var i = 0; i < buttonRefs.Length; i++)
            {
                Vector3 targetPost;

                var sizeDelta = new Vector2(m_ButtonScaler.newButtonSize.x, m_ButtonScaler.newButtonSize.y);

                Vector3 position = transform.position;

                targetPost.x = linSpawner.direction.x * ((i + linSpawner.buttonNumOffset) * (sizeDelta.x + linSpawner.buttonSpacing)) + position.x;
                targetPost.y = linSpawner.direction.y * ((i + linSpawner.buttonNumOffset) * (sizeDelta.y + linSpawner.buttonSpacing)) + position.y;

                targetPost.z = 0;

                if (buttonRefs[i].transform is RectTransform buttonRect)
                {
                    buttonRect.sizeDelta = sizeDelta;
                    buttonRect.position = Vector3.Lerp(buttonRect.position, targetPost, revealSettings.translateSmooth * Time.deltaTime);
                }
            }
        }
        private void RevealLinearlyFade()
        {
            for (var i = 0; i < buttonRefs.Length; i++)
            {
                Vector3 targetPos;

                var sizeDelta = new Vector2(m_ButtonScaler.newButtonSize.x, m_ButtonScaler.newButtonSize.y);

                Vector3 position = transform.position;

                targetPos.x = linSpawner.direction.x * ((i + linSpawner.buttonNumOffset) * (sizeDelta.x + linSpawner.buttonSpacing)) + position.x;
                targetPos.y = linSpawner.direction.y * ((i + linSpawner.buttonNumOffset) * (sizeDelta.y + linSpawner.buttonSpacing)) + position.y;

                targetPos.z = 0;

                ButtonFader previousButtonFader = i > 0 ? buttonRefs[i - 1] : null;
                ButtonFader buttonFader = buttonRefs[i];
                
                if (buttonFader.transform is RectTransform buttonRect) buttonRect.sizeDelta = sizeDelta;

                void FadeAndPosition()
                {
                    buttonFader.transform.position = targetPos;
                    if (buttonFader) buttonFader.Fade(revealSettings.fadeSmooth);
                }

                if (previousButtonFader)
                {
                    if (previousButtonFader.faded) FadeAndPosition();
                }
                else FadeAndPosition();
            }
        }

        private void SetButtonRefsEnable(bool enable) => buttonRefs.ForEach(buttonRef => buttonRef.gameObject.SetActive(enable));
    }
}
