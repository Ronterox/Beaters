using System;
using DG.Tweening;
using Plugins.Audio;
using Plugins.Properties;
using Plugins.Tools;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class LevelLoadManager : Singleton<LevelLoadManager>
    {
        public float transitionDuration;
        [Scene]
        public string arrowGameplayScene, mainMenuScene;
        public CanvasGroup transitionPanel;

        [Header("Sfx")]
        public AudioClip loadSfx;

        private void Start() => OpenTransition(transitionDuration);

        private static void FadeTransitionPanel(float target, float duration, Action onEnd = null)
        {
            CanvasGroup canvasGroup = Instance.transitionPanel;

            if (!canvasGroup) return;

            canvasGroup.alpha = Mathf.Abs(target - 1f);
            
            m_Instance.transitionPanel.DOFade(target, duration).OnComplete(() =>
            {
                canvasGroup.gameObject.SetActive(canvasGroup.alpha != 0);
                onEnd?.Invoke();
            });
        }

        public static void OpenTransition(float duration) => FadeTransitionPanel(0f, duration);

        public static void CloseTransition(float duration, Action onEnd) => FadeTransitionPanel(1f, duration, onEnd);

        public static void LoadSceneWithTransition(string scene, float duration) => CloseTransition(duration, () =>
        {
            SoundManager.Instance.PlayNonDiegeticRandomPitchSound(m_Instance.loadSfx);
            SceneManager.LoadScene(scene);
        });

        public static void LoadArrowGameplayScene() => LoadSceneWithTransition(m_Instance.arrowGameplayScene, m_Instance.transitionDuration);

        public static void LoadMainMenu() => LoadSceneWithTransition(m_Instance.mainMenuScene, m_Instance.transitionDuration);
    }
}
