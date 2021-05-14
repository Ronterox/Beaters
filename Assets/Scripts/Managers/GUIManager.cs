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

            foreach (GUIImage guiImage in images) guiImage.image.color = palette.GetColor(guiImage.paletteColor);

            if(playButton) playButton.sprite = character.playButton;
            if(gachaLogo) gachaLogo.sprite = character.gachaButton;
            if(mapCreator) mapCreator.sprite = character.mapCreator;
            backgroundImage.sprite = character.backgroundImage;

            foreach (TMP_Text text in textsOfTheUI)
            {
                text.font = character.font;
            }
        }
    }
}
