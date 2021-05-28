using Managers;
using Plugins.Properties;
using Plugins.Tools;
using UnityEngine;
using Utilities;

namespace General
{
    public class TutorialButtons : MonoBehaviour
    {
        [Scene]
        public string tutorialScene;
        public CanvasGroup[] canvasGroups;
        [Space]
        public GameObject popUp;
#if UNITY_EDITOR
        [Header("Editor Only")]
        public bool seeTutorial;
#endif
        public const string FIRST_TIME_KEY = "First Gameplay";

        private void Start()
        {
#if UNITY_EDITOR
            if (!seeTutorial) return;
#endif
            if (!PlayerPrefs.HasKey(FIRST_TIME_KEY) || PlayerPrefs.GetInt(FIRST_TIME_KEY) != 1)
            {
                LevelLoadManager.LoadSceneWithTransition(tutorialScene, LevelLoadManager.Instance.transitionDuration);
                return;
            }

            if (DataManager.Instance.CharacterCount > 0) return;

            var dialoguesMessage = Instantiate(popUp, transform).GetComponent<DialoguesMessage>();
            dialoguesMessage.AddDialogues("Congratulations, now you now how to play", 
                                          "Now, you will have to get your first character!", 
                                          "Try going to the gacha screen to get a new character!");

            canvasGroups.ForEach(group =>
            {
                group.interactable = false;
                group.alpha = .5f;
            });
        }
    }
}
