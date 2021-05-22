using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

namespace UI
{
    public class SliderValueToText : MonoBehaviour
    {
        public Slider sliderMaster, sliderSFX, sliderMusic, sliderOffset;
        public TextMeshProUGUI textMaster, textSFX, textMusic, textOffset;

        void Start()
        {
            ShowSliderValue();
        }

        public void ShowSliderValue()
        {
            string sliderMessageMaster = "Master Volume: " + sliderMaster.value;
            string sliderMessageSFX = "SFX Volume: " + sliderSFX.value;
            string sliderMessageMusic = "Music Volume: " + sliderMusic.value;
            string sliderMessageOffset = "Audio offset: " + sliderOffset.value + " ms";
            textMaster.text = sliderMessageMaster;
            textSFX.text = sliderMessageSFX;
            textMusic.text = sliderMessageMusic;
            textOffset.text = sliderMessageOffset;
        }
    }
}