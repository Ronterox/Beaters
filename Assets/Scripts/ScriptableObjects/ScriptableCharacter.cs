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
    public ScriptableItem[] items;
    public int hp;
    public float multiplier;

    public Sprite PlayButton, GatchaButton, MapCreator;
    public ushort ID { get; private set; }

    private void Awake() => ID = (ushort)characterName.GetHashCode();
}
[System.Serializable]
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
