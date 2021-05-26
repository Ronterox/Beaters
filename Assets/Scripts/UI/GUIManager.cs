using System.Linq;
using Managers;
using Plugins.Tools;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [System.Serializable]
    public struct GUIImage
    {
        public enum PaletteColor { MainColor, SecondaryColor, complementaryColor1, complementaryColor2 }

        public Image image;
        public PaletteColor paletteColor;
    }
    public class GUIManager : MonoBehaviour
    {
        public GUIImage[] images;
        [Space]
        public ScriptableCharacter[] characters;
        [Space]
        public Image backgroundImage;
        public Image[] playButtons;
        public Image gachaLogo, mapCreator, exitGame;
        [Space]
        public TMP_Text[] textsOfTheUI;
        public Image[] buttonsToChange;

        private void Start()
        {
            if (DataManager.Instance.CharacterCount < 1) return;

            SetCharacterGUI(GetCharacter());
        }

        private ScriptableCharacter GetCharacter()
        {
            ScriptableCharacter character = GameManager.GetCharacter();
            if (character) return character;

            ushort randomCharacter = DataManager.GetCharactersIds().GetRandom();

            character = characters.First(c => c.ID == randomCharacter);
            GameManager.PutCharacter(character);

            return character;
        }

        public void SetCharacterGUI(ScriptableCharacter character)
        {
            Palette palette = character.colorPalette;

            SetSprite(gachaLogo, character.gachaButton);
            SetSprite(mapCreator, character.mapCreator);
            SetSprite(backgroundImage, character.backgroundImage);
            SetSprite(exitGame, character.exitButton);

            images.ForEach(image => image.image.color = palette.GetColor(image.paletteColor));

            playButtons.ForEach(image => SetSprite(image, character.playButton));
            
            buttonsToChange.ForEach(button => button.sprite = character.buttonLayout);
            
            textsOfTheUI.ForEach(text => text.font = character.font);
        }

        private void SetSprite(Image image, Sprite sprite)
        {
            if (sprite && image) image.sprite = sprite;
        }
    }
}
