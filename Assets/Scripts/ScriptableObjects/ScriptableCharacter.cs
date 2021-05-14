using General;
using Managers;
using UnityEngine;
using TMPro;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Character", menuName = "Characters/New character")]
    public class ScriptableCharacter : IdentifiedScriptable
    {
        public string characterName;
        [TextArea] public string description;

        [Header("Stats")]
        public int hp;
        public float multiplier;
        public ScriptableSkill activeSkill, passiveSkill;

        [Header("Visuals")]
        public Sprite backgroundImage;
        public Palette colorPalette;
        public Sprite[] sprites;
        [Space]
        public TMP_FontAsset font;
        public Sprite playButton, gachaButton, mapCreator, exitButton;
        public Sprite emptyStar, fullStar;
    }

    [System.Serializable]
    public struct Palette
    {
        public Color mainColor, secondaryColor;
        public Color complementaryColor1, complementaryColor2;

        /// <summary>
        /// Obtains a the color version of the palette color enum selected
        /// </summary>
        /// <param name="paletteColor">color member of the palette</param>
        /// <returns></returns>
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
