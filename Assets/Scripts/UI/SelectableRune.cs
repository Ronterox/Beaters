using Managers;
using Plugins.UI;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SelectableRune : ImageText
    {
        [Space]
        public Button button;
        public void Set(ScriptableRune scriptableRune)
        {
            if (!DataManager.ContainsRune(scriptableRune.ID))
            {
                canvasGroup.alpha = .5f;
                canvasGroup.interactable = false;
            }

            image.sprite = scriptableRune.runeSprite;
            tmpText.text = scriptableRune.effectDescription;
            
            gameObject.SetActive(true);
        }
    }
}
