using UnityEngine;

namespace ScriptableObjects
{

[CreateAssetMenu(fileName = "New Character", menuName = "Characters/New character")]
public class Character : ScriptableObject
{
    public string characterName;
    public Skill[] skill;
    public Palette colorPalette;
    public Sprite[] sprites;
    public Item[] items;
    public float hp;
    public int level;
    public ushort ID { get; private set; }

    private void Awake() => ID = (ushort)characterName.GetHashCode();
}
public struct Palette
        {
        public Color mainColor, secondaryColor;
        public Color complementaryColor1, complementaryColor2;

        public Color GetColor(GUIImage.ColorPosition colorPosition)
        {
            switch (colorPosition)
            {
                case GUIImage.ColorPosition.MainColor:
                    return mainColor;

                case GUIImage.ColorPosition.SecondaryColor:
                    return secondaryColor;

                case GUIImage.ColorPosition.complementaryColor1:
                    return complementaryColor1;
                
                case GUIImage.ColorPosition.complementaryColor2:
                    return complementaryColor2;
                    
                default:
                    return Color.red;

            }
        }

}

}
