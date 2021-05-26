using Managers;
using Plugins.Tools;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [System.Serializable]
    public struct GameplayBeatUI
    {
        public SpriteRenderer renderer;
        public GUIImage.PaletteColor color;
        public bool changeSprite;
    }
    
    public class GameplayUIManager : MonoBehaviour
    {
        public GameplayBeatUI[] gameplayUi;
        public Image skillImage;
        [Space]
        public TMP_Text[] texts;

        private void Start()
        {
            ScriptableCharacter character = GameManager.GetCharacter();
            Palette palette = character.colorPalette;

            gameplayUi.ForEach(ui =>
            {
                if(ui.changeSprite) ui.renderer.sprite = character.noteSprite;
                ui.renderer.color = palette.GetColor(ui.color);
            });
            
            texts.ForEach(text => text.font = character.font);

            skillImage.sprite = character.activeSkill.skillImage;
        }
    }
}
