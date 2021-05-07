using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    //TODO: Change the script to only allow owned characters to be chosen

    [System.Serializable]
    public struct GUIImage
    {
        public enum PaletteColor { MainColor, SecondaryColor, complementaryColor1, complementaryColor2 }

        public enum ButtonsOnScreen { PlayButton, GachaButton, MapCreatorButton }

        public Image image;
        public PaletteColor paletteColor;
    }


    public class GUIManager : MonoBehaviour
    {
        public GUIImage[] images;
        public ScriptableCharacter[] characters;
        public Image playButton, gachaLogo, mapCreator;

        private void Start() => SetCharacterGUI(characters[Random.Range(0, characters.Length)]);

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
