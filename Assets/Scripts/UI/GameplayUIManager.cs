using Managers;
using Plugins.Properties;
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
        [Space]
        public bool changeAlpha;
        [ConditionalHide("changeAlpha")]
        public float newAlpha;
    }

    public class GameplayUIManager : MonoBehaviour
    {
        public GUIImage[] images;
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
                ui.renderer.color = palette.GetColor(ui.color);
                if (ui.changeSprite) ui.renderer.sprite = character.noteSprite;
                if (ui.changeAlpha) ui.renderer.SetAlpha(ui.newAlpha);
            });

            images.ForEach(image => image.SetColor(palette));

            texts.ForEach(text => text.font = character.font);

            skillImage.sprite = character.activeSkill.skillImage;
        }
    }
}
