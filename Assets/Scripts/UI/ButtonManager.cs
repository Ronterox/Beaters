using Plugins.Properties;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class ButtonManager : MonoBehaviour
    {
        [Scene]
        public string mainMenuScene;

        public void LoadScene(int numberOfTheScene)
        {
            SceneManager.LoadScene(numberOfTheScene);
        }
        public void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif

            Application.Quit();
        }

        public void LoadMainMenu()
        {
            SceneManager.LoadScene(mainMenuScene);
        }
        public void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        public void PastScene(){
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex-1);
        }
    }
}
