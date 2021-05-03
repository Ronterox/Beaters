using Plugins.Tools;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Plugins.GUI
{
    public class PauseMenu : Singleton<PauseMenu>
    {
        public GameObject resumeGame, quitGame, mainMenu;
        [SerializeField]
        private GameObject pauseMenuUI;

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Escape)) return;
            //if (//GameManager.Instance.gameIsPaused) Resume();
            else Pause();
        }

        /// <summary>
        /// Resumes the game
        /// </summary>
        public void Resume()
        {
            Time.timeScale = 1f;
            pauseMenuUI.SetActive(false);
            //GameManager.Instance.gameIsPaused = false;
        }

        /// <summary>
        /// Pauses the in game time
        /// </summary>
        public void Pause()
        {
            Time.timeScale = 0f;
            pauseMenuUI.SetActive(true);
            //GameManager.Instance.gameIsPaused = true;
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(resumeGame);
        }

        /// <summary>
        /// Takes you to main menu
        /// </summary>
        public void MainMenu()
        {
            Resume();
            //LevelLoadManager.Instance.LoadScene("MAIN MENU");
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(mainMenu);
        }

        /// <summary>
        /// Quits The Game
        /// </summary>
        public void QuitGame()
        {
            //LevelLoadManager.Instance.QuitGame();
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(quitGame);
        }
    }
}
