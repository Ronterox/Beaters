using System.Linq;
using Plugins.Tools;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Managers
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
        public Image playButton, gachaLogo, mapCreator, exitGame;
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

            return characters.First(c => c.ID == randomCharacter);
        }

        private void SetCharacterGUI(ScriptableCharacter character)
        {
            Palette palette = character.colorPalette;

            if (character.usePrimaryColorInButtons) images.ForEach(image => image.image.color = palette.GetColor(image.paletteColor));
            else images.ForEach(image => image.image.color = Color.white);

            SetSprite(playButton, character.playButton);
            SetSprite(gachaLogo, character.gachaButton);
            SetSprite(mapCreator, character.mapCreator);
            SetSprite(backgroundImage, character.backgroundImage);
            SetSprite(exitGame, character.exitButton);

            buttonsToChange?.ForEach(button => button.sprite = character.buttonLayout);
            textsOfTheUI.ForEach(text => text.font = character.font);
        }

        private void SetSprite(Image image, Sprite sprite)
        {
            if (sprite && image) image.sprite = sprite;
        }
    }
}
