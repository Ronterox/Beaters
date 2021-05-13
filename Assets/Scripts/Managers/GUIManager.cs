using System.Linq;
using Plugins.Tools;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

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
        public Image playButton, gachaLogo, mapCreator;

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

            playButton.sprite = character.playButton;
            gachaLogo.sprite = character.gachaButton;
            mapCreator.sprite = character.mapCreator;
        }
    }
}
