using System.Collections.Generic;
using Plugins.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Plugins.GUI
{
    [System.Serializable]
    public struct Settings
    {
        public float generalVolume;
        public float uiVolume;
        public float sfxVolume;
        public float musicVolume;

        public bool fullScreen;
        public int resolution;
    }

    public class SettingsMenu : MonoBehaviour
    {
        private Settings settings;

        private Resolution[] resolutions;

        [SerializeField] private Slider generalVolume;
        [SerializeField] private Slider uiVolume;
        [SerializeField] private Slider sfxVolume;
        [SerializeField] private Slider musicVolume;

        [SerializeField] private Toggle fullscreenToggle;

        [SerializeField] private TMP_Dropdown resolutionDropdown;

        private const string SAVED_FILENAME = "settings.cfg";
        private const string SAVED_FOLDERNAME = "SavedStates";
        private void Start()
        {
            SetSystemResolutions();
            CheckForSavedSettings();
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Finds and sets the available Resolution Options for the user
        /// </summary>
        private void SetSystemResolutions()
        {
            resolutions = Screen.resolutions;

            var resolutionsList = new List<string>();
            var resolutionIndex = 0;

            foreach (Resolution resolution in resolutions)
            {
                resolutionsList.Add(resolution.width + " x " + resolution.height);
                if (resolution.Equals(Screen.currentResolution)) resolutionIndex = resolutionsList.Count - 1;
            }
            resolutionDropdown.AddOptions(resolutionsList);
            resolutionDropdown.value = resolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }

        /// <summary>
        /// Checks if there is saved settings, else it creates it
        /// </summary>
        private void CheckForSavedSettings()
        {
            if (SaveLoadManager.SaveExists(SAVED_FILENAME, SAVED_FOLDERNAME))
            {
                settings = SaveLoadManager.Load<Settings>(SAVED_FILENAME, SAVED_FOLDERNAME);
#if !UNITY_EDITOR
                SetResolution(settings.resolution);
                SetFullscreen(settings.fullScreen);
#endif
                UpdateGameObjects();
            }
            else
            {
                SoundManager soundManager = SoundManager.Instance;
                settings = new Settings
                {
                    fullScreen = fullscreenToggle.isOn,

                    generalVolume = soundManager.generalVolume,
                    musicVolume = soundManager.musicVolume,
                    sfxVolume = soundManager.sfxVolume,
                    uiVolume = soundManager.uiVolume
                };
                SaveSettings();
            }
        }

        /// <summary>
        /// Updates the visual values for each gameObject
        /// </summary>
        private void UpdateGameObjects()
        {
            fullscreenToggle.isOn = settings.fullScreen;

            generalVolume.value = settings.generalVolume;
            uiVolume.value = settings.uiVolume;
            sfxVolume.value = settings.sfxVolume;
            musicVolume.value = settings.musicVolume;

            resolutionDropdown.value = settings.resolution;
        }

        /// <summary>
        /// Sets the general volume of the game
        /// </summary>
        /// <param name="volume"></param>
        public void SetGeneralVolume(float volume) => SoundManager.Instance.SetVolume(settings.generalVolume = volume);

        /// <summary>
        /// Sets the music volume of the game
        /// </summary>
        /// <param name="volume"></param>
        public void SetMusicVolume(float volume) => SoundManager.Instance.SetMusicVolume(settings.musicVolume = volume);

        /// <summary>
        /// Sets the sound effects volume of the game
        /// </summary>
        /// <param name="volume"></param>
        public void SetSFXVolume(float volume) => SoundManager.Instance.SetSfxVolume(settings.sfxVolume = volume);

        /// <summary>
        /// Sets the ui volume of the game
        /// </summary>
        /// <param name="volume"></param>
        public void SetUIVolume(float volume) => SoundManager.Instance.SetUIVolume(settings.uiVolume = volume);

        /// <summary>
        /// Sets the resolution available at the specific position on the array of resolutions
        /// </summary>
        /// <param name="resolutionIndex"></param>
        public void SetResolution(int resolutionIndex)
        {
            Resolution currentResolution = resolutions[resolutionIndex];
            settings.resolution = resolutionIndex;
            Screen.SetResolution(currentResolution.width, currentResolution.height, Screen.fullScreen);
        }

        private void OnDisable() => SaveSettings();

        /// <summary>
        /// Sets the application to fullscreen or not fullscreen
        /// </summary>
        /// <param name="isFullscreen"></param>
        public void SetFullscreen(bool isFullscreen) => Screen.fullScreen = settings.fullScreen = isFullscreen;

        private void SaveSettings() => SaveLoadManager.Save(settings, SAVED_FILENAME, SAVED_FOLDERNAME);
    }
}
