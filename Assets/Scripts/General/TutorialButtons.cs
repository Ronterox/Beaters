using Managers;
using Plugins.Properties;
using Plugins.Tools;
using Plugins.UI;
using TMPro;
using UnityEngine;

namespace General
{
    public class TutorialButtons : MonoBehaviour
    {
        [Scene]
        public string tutorialScene;
        public CanvasGroup[] canvasGroups;
        [Space]
        public GameObject popUp;
        
        public const string FIRST_TIME_KEY = "First Gameplay";

        private void Start()
        {
            if (!PlayerPrefs.HasKey(FIRST_TIME_KEY) || PlayerPrefs.GetInt(FIRST_TIME_KEY) != 1)
            {
                LevelLoadManager.LoadSceneWithTransition(tutorialScene, LevelLoadManager.Instance.transitionDuration);
                return;
            }

            if (DataManager.Instance.CharacterCount > 0) return;

            Instantiate(popUp).GetComponent<ImageText>().tmpText.text = "Congratulations, now you now how to play, you will have to get a new character"
                                                               + "\nTry going to the gacha screen to get a new character!";
                
            canvasGroups.ForEach(group =>
            {
                group.interactable = false;
                group.alpha = .5f;
            });
        }
    }
}
