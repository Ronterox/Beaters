using DG.Tweening;
using Plugins.Properties;
using Plugins.Tools;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class LevelLoadManager : Singleton<LevelLoadManager>
    {
        [Scene]
        public string arrowGameplayScene;
        public CanvasGroup transitionPanel;

        private const float DEFAULT_DURATION = 1f;

        private void Start() => OpenTransition(DEFAULT_DURATION);

        private static void FadeTransitionPanel(float target, float duration)
        {
            CanvasGroup canvasGroup = Instance.transitionPanel;
            
            if (!canvasGroup) return;
            
            canvasGroup.alpha = Mathf.Abs(target - 1f);
            Instance.transitionPanel.DOFade(target, duration).OnComplete(() => canvasGroup.gameObject.SetActive(canvasGroup.alpha != 0));
        }

        public static void OpenTransition(float duration) => FadeTransitionPanel(0f, duration);

        public static void CloseTransition(float duration) => FadeTransitionPanel(1f, duration);

        public static void LoadSceneWithTransition(string scene, float duration)
        {
            SceneManager.LoadScene(scene);
            CloseTransition(duration);
        }

        public static void LoadArrowGameplayScene() => LoadSceneWithTransition(Instance.arrowGameplayScene, DEFAULT_DURATION);
    }
}
