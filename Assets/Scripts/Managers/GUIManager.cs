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

            if(character.usePrimaryColorInButtons) foreach (GUIImage guiImage in images) guiImage.image.color = palette.GetColor(guiImage.paletteColor);
            else foreach (GUIImage guiImage in images) guiImage.image.color = Color.white;

            SetSprite(playButton, character.playButton);
            SetSprite(gachaLogo, character.gachaButton);
            SetSprite(mapCreator, character.mapCreator);
            SetSprite(backgroundImage, character.backgroundImage);
            SetSprite(exitGame, character.exitButton);

            foreach(Image button in buttonsToChange){
                button.sprite = character.buttonLayout;
            }
            textsOfTheUI.ForEach(text => text.font = character.font);
        }

        private void SetSprite(Image image, Sprite sprite)
        {
            if (sprite && image) image.sprite = sprite;
        }
    }
}
