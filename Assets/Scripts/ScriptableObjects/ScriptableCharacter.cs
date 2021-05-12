using Managers;
using Plugins.Tools;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Character", menuName = "Characters/New character")]
    public class ScriptableCharacter : ScriptableObject
    {
        public string characterName;
        public ScriptableSkill[] skill;
        public Palette colorPalette;
        public Sprite[] sprites;
        public int hp;
        public float multiplier;

        public Sprite playButton, gachaButton, mapCreator;
        public ushort ID => characterName.GetHashCodeUshort();
    }

    [System.Serializable]
    public struct Palette
    {
        public Color mainColor, secondaryColor;
        public Color complementaryColor1, complementaryColor2;

        public Color GetColor(GUIImage.PaletteColor paletteColor) =>
            paletteColor switch
            {
                GUIImage.PaletteColor.MainColor => mainColor,
                GUIImage.PaletteColor.SecondaryColor => secondaryColor,
                GUIImage.PaletteColor.complementaryColor1 => complementaryColor1,
                GUIImage.PaletteColor.complementaryColor2 => complementaryColor2,
                _ => Color.red
            };

    }

}
