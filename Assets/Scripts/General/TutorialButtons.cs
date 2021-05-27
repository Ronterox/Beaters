using Plugins.Properties;
using Plugins.Tools;
using UnityEngine;

namespace General
{
    public class TutorialButtons : MonoBehaviour
    {
        [Scene]
        public string tutorialScene;
        public CanvasGroup[] canvasGroups;

        private void Start()
        {
            const string firstTimeKey = "First Gameplay";
            return;
            if (!PlayerPrefs.HasKey(firstTimeKey) || PlayerPrefs.GetInt(firstTimeKey) != 1)
            {
                //LevelLoadManager.LoadSceneWithTransition(tutorialScene, LevelLoadManager.Instance.transitionDuration);
                //return;
            }

            canvasGroups.ForEach(group =>
            {
                group.interactable = false;
                group.alpha = .5f;
            });
        }
    }
}
