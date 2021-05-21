using System.Collections.Generic;
using Plugins.Audio;
using Plugins.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Plugins.GUI
{
    [System.Serializable]
    public class Settings
    {
        public float generalVolume, sfxVolume, musicVolume;

        public bool fullScreen;
        public int resolution;
    }

    public class SettingsMenu : MonoBehaviour
    {
        public Settings settings;
        private Resolution[] m_Resolutions;
        
        public Slider generalVolume, sfxVolume, musicVolume;

        [Space]
        public Toggle fullscreenToggle;
        public TMP_Dropdown resolutionDropdown;

        private const string SAVED_FILENAME = "settings.cfg";
        private void Start() => SetSystemResolutions();

        /// <summary>
        /// Finds and sets the available Resolution Options for the user
        /// </summary>
        private void SetSystemResolutions()
        {
            m_Resolutions = Screen.resolutions;

            var resolutionsList = new List<string>();
            var resolutionIndex = 0;

            foreach (Resolution resolution in m_Resolutions)
            {
                resolutionsList.Add(resolution.width + " x " + resolution.height);
                if (resolution.Equals(Screen.currentResolution)) resolutionIndex = resolutionsList.Count - 1;
            }
            resolutionDropdown.AddOptions(resolutionsList);
            resolutionDropdown.value = resolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }

        /// <summary>
        /// Sets the general volume of the game
        /// </summary>
        /// <param name="volume"></param>
        public void SetGeneralVolume(float volume) => SoundManager.Instance.SetMasterVolume(settings.generalVolume = volume);

        /// <summary>
        /// Sets the music volume of the game
        /// </summary>
        /// <param name="volume"></param>
        public void SetMusicVolume(float volume) => SoundManager.Instance.SetMusicVolume(settings.musicVolume = volume);

        /// <summary>
        /// Sets the sound effects volume of the game
        /// </summary>
        /// <param name="volume"></param>
        public void SetSFXVolume(float volume) => SoundManager.Instance.SetSFXVolume(settings.sfxVolume = volume);

        /// <summary>
        /// Sets the resolution available at the specific position on the array of m_Resolutions
        /// </summary>
        /// <param name="resolutionIndex"></param>
        public void SetResolution(int resolutionIndex)
        {
            Resolution currentResolution = m_Resolutions[resolutionIndex];
            settings.resolution = resolutionIndex;
            Screen.SetResolution(currentResolution.width, currentResolution.height, Screen.fullScreen);
        }

        private void OnDisable() => SaveSettings();

        /// <summary>
        /// Sets the application to fullscreen or not fullscreen
        /// </summary>
        /// <param name="isFullscreen"></param>
        public void SetFullscreen(bool isFullscreen) => Screen.fullScreen = settings.fullScreen = isFullscreen;

        private void SaveSettings() => SaveLoadManager.Save(settings, SAVED_FILENAME);
    }
}
