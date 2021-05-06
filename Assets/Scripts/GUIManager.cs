using UnityEngine;
using UnityEngine.UI;
using ScriptableObjects;

public struct GUIImage
{
    public enum ColorPosition { MainColor, SecondaryColor, complementaryColor1, complementaryColor2 }
    public Image image;
    public ColorPosition color;
}


public class GUIManager : MonoBehaviour
{
    public GUIImage[] images;
    public Character character;

    private void Start()
    {
        Palette palette = character.colorPalette;
        foreach (GUIImage guiImage in images)
        {
            guiImage.image.color = palette.GetColor(guiImage.color);
        }
    }
}