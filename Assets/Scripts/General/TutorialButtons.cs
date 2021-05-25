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
            if (DataManager.Instance.CharacterCount >= 1) return;

            canvasGroups.ForEach(group =>
            {
                group.interactable = false;
                group.alpha = .5f;
            });
        }
    }
}
