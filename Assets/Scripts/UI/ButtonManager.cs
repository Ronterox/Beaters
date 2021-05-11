using Plugins.GUI;
using Plugins.Properties;
using Plugins.Tools;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    [System.Serializable]
    public struct MenuButton
    {
        public Button selectableButton;
        [Scene]
        public string scene;
    }

    public class ButtonManager : MonoBehaviour
    {
        public MenuButton[] menuButtons;

        private void Start() => menuButtons.ForEach(button => button.selectableButton.onClick.AddListener(() => LoadScene(button.scene)));

        public void LoadScene(string sceneName) => SceneManager.LoadScene(sceneName);

        public void ReloadScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        public void PastScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);

        public void NextScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        public void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
