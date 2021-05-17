using Managers;
using Plugins.UI;
using ScriptableObjects;
using UI;
using UnityEngine;

namespace Utilities
{
    public class RuneScriptableSelector : MonoBehaviour
    {
        public Transform content;
        public GameObject template;
        [Space]
        public ScriptableRune[] gameRunes;
        public ImageText selectedRune;

        private void Start()
        {
            template.SetActive(false);
            
            foreach (ScriptableRune scriptableRune in gameRunes)
            {
                var rune = Instantiate(template, content).GetComponent<SelectableRune>();
                rune.Set(scriptableRune);
                
                rune.button.onClick.AddListener(() =>
                    {
                        content.gameObject.SetActive(false);
                        GameManager.PutRune(scriptableRune);
                        selectedRune.Set(scriptableRune.runeName, scriptableRune.runeSprite);
                    });
            }
        }
    }
}
