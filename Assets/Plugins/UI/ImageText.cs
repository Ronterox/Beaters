using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Plugins.UI
{
    public class ImageText : MonoBehaviour
    {
        public CanvasGroup canvasGroup;
        public TMP_Text tmpText;
        public Image image;

        public void Set(string text, Sprite sprite)
        {
            tmpText.text = text;
            image.sprite = sprite;
        }
    }
}
