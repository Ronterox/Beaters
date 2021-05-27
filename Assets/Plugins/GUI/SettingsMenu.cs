using System.Collections.Generic;
using System.Diagnostics;
using Plugins.Audio;
using Plugins.Properties;
using Plugins.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Plugins.GUI
{
    [System.Serializable]
    public class Settings
    {
        public float generalVolume = 1, sfxVolume = 1, musicVolume = 1;

        public bool fullScreen = true;
        public int resolution = -1, tapOffset;
    }

    public class SettingsMenu : MonoBehaviour
    {
        [ReadOnly, SerializeField]
        private Settings m_Settings = new Settings();
        [ReadOnly]
        private Resolution[] m_Resolutions;

        public Slider generalVolume, sfxVolume, musicVolume;
        public Slider tapOffsetSlider;

        [Space]
        public Button tapOffsetButton;

        [Space]
        public Toggle fullscreenToggle;
        public TMP_Dropdown resolutionDropdown;
        [Space]
        public AudioClip metronomeAudioClip;

        public const string SAVED_FILENAME = "settings.cfg";
        private Stopwatch m_Stopwatch = new Stopwatch();

        private void Start()
        {
            SetSystemResolutions();
            SetOldSettings();

            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
            resolutionDropdown.onValueChanged.AddListener(SetResolution);

            generalVolume.onValueChanged.AddListener(SetGeneralVolume);
            sfxVolume.onValueChanged.AddListener(SetSFXVolume);
            musicVolume.onValueChanged.AddListener(SetMusicVolume);

            tapOffsetButton.onClick.AddListener(StartOffSetCalculation);
        }

        private void OnDestroy() => SaveSettings();

        private void Update()
        {
            if (m_Stopwatch.IsRunning && Input.GetMouseButtonDown(0)) CalculateOffSet();
        }

        private void CalculateOffSet()
        {
            const float expectedMs = 2000; //Perfect timed total 4 beats 
            m_Stopwatch.Stop();

            float resultOffset = m_Stopwatch.ElapsedMilliseconds - expectedMs;
            tapOffsetSlider.value = resultOffset;

            SoundManager.Instance.StopAllSfx();
        }

        private void StartOffSetCalculation()
        {
            SoundManager.Instance.PlayNonDiegeticSound(metronomeAudioClip);
            m_Stopwatch.Restart();
        }

        private void SetOldSettings()
        {
            if (SaveLoadManager.SaveExists(SAVED_FILENAME)) m_Settings = SaveLoadManager.Load<Settings>(SAVED_FILENAME);

            SetFullscreen(fullscreenToggle.isOn = m_Settings.fullScreen);

            SetGeneralVolume(generalVolume.value = m_Settings.generalVolume);
            SetMusicVolume(musicVolume.value = m_Settings.musicVolume);
            SetSFXVolume(sfxVolume.value = m_Settings.sfxVolume);

            if (m_Settings.resolution > -1) SetResolution(resolutionDropdown.value = m_Settings.resolution);
        }

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
        public void SetGeneralVolume(float volume) => SoundManager.Instance.SetMasterVolume(m_Settings.generalVolume = volume);

        /// <summary>
        /// Sets the music volume of the game
        /// </summary>
        /// <param name="volume"></param>
        public void SetMusicVolume(float volume) => SoundManager.Instance.SetMusicVolume(m_Settings.musicVolume = volume);

        /// <summary>
        /// Sets the sound effects volume of the game
        /// </summary>
        /// <param name="volume"></param>
        public void SetSFXVolume(float volume) => SoundManager.Instance.SetSFXVolume(m_Settings.sfxVolume = volume);

        /// <summary>
        /// Sets the resolution available at the specific position on the array of m_Resolutions
        /// </summary>
        /// <param name="resolutionIndex"></param>
        public void SetResolution(int resolutionIndex)
        {
            Resolution currentResolution = m_Resolutions[resolutionIndex];
            m_Settings.resolution = resolutionIndex;
            Screen.SetResolution(currentResolution.width, currentResolution.height, Screen.fullScreen);
        }

        /// <summary>
        /// Sets the application to fullscreen or not fullscreen
        /// </summary>
        /// <param name="isFullscreen"></param>
        public void SetFullscreen(bool isFullscreen) => Screen.fullScreen = isFullscreen;

        private void SaveSettings()
        {
            SoundManager soundManager = SoundManager.Instance;

            m_Settings.fullScreen = Screen.fullScreen;

            m_Settings.generalVolume = soundManager.MasterVolume;
            m_Settings.musicVolume = soundManager.MusicVolume;
            m_Settings.sfxVolume = soundManager.SFXVolume;

            m_Settings.tapOffset = (int)tapOffsetSlider.value;

            SaveLoadManager.Save(m_Settings, SAVED_FILENAME);
        }
    }

}
