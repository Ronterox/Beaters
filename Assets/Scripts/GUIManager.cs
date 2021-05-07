using UnityEngine;
using UnityEngine.UI;
using ScriptableObjects;

//TODO: Change the script to only allow owned characters to be chosen

[System.Serializable]
public struct GUIImage
{
    public enum ColorPosition { MainColor, SecondaryColor, complementaryColor1, complementaryColor2 }
    public enum ButtonsOnScreen { PlayButton, GachaButton, MapCreatorButton }
    public Image image;
    public ColorPosition color;
}


public class GUIManager : MonoBehaviour
{
    public GUIImage[] images;
    public ScriptableCharacter[] characters;
    public Image playButton;
    public Image gachaLogo;
    public Image mapCreator;

    private void Start()
    {
        ScriptableCharacter character = characters[Random.Range(0,characters.Length)];
        Palette palette = character.colorPalette;
        foreach (GUIImage guiImage in images)
        {
            guiImage.image.color = palette.GetColor(guiImage.color);
        }
        playButton.sprite = character.PlayButton;
        gachaLogo.sprite = character.sprites[0];
        mapCreator.sprite = character.MapCreator;

    }
}