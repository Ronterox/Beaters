using Managers;
using Plugins.Tools;
using UnityEngine;

namespace General
{
    public class TutorialButtons : MonoBehaviour
    {
        public CanvasGroup[] canvasGroups;

        private void Start()
        {
            if (DataManager.Instance.CharacterCount < 1) canvasGroups.ForEach(button =>
            {
                button.interactable = false;
                button.alpha = .5f;
            });
        }
    }
}
