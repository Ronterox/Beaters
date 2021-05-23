using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI
{
    public class SliderValueToText : MonoBehaviour
    {
        [Header("Elements")]
        public Slider sliderMaster;
        public Slider sliderSFX, sliderMusic, sliderOffset;
        [Header("Text")]
        public TMP_Text textMaster;
        public TMP_Text textSFX, textMusic, textOffset;

        private void Start() => ShowSliderValue();

        public void ShowSliderValue()
        {
            textMaster.text = "Master Volume: " + sliderMaster.value;
            textSFX.text = "SFX Volume: " + sliderSFX.value;
            textMusic.text = "Music Volume: " + sliderMusic.value;
            textOffset.text = "Audio offset: " + sliderOffset.value + " ms";
        }
    }
}