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
        public ScriptableCharacter[] characters;
        public Image playButton, gachaLogo, mapCreator, exitGame;
        public Image backgroundImage;
        public TMP_Text[] textsOfTheUI;

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

            images.ForEach(image => image.image.color = palette.GetColor(image.paletteColor));

            SetSprite(playButton, character.playButton);
            SetSprite(gachaLogo, character.gachaButton);
            SetSprite(mapCreator, character.mapCreator);
            SetSprite(backgroundImage, character.backgroundImage);
            SetSprite(exitGame, character.exitButton);

            textsOfTheUI.ForEach(text => text.font = character.font);
        }

        private void SetSprite(Image image, Sprite sprite)
        {
            if (sprite && image) image.sprite = sprite;
        }
    }
}
