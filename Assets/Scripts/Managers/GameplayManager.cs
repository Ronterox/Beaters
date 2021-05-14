using Core.Arrow_Game;
using General;
using Plugins.Tools;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using Random = UnityEngine.Random;

namespace Managers
{
    public class GameplayManager : Singleton<GameplayManager>
    {
        public MapScroller mapScroller;

        [Header("Config")]
        public Canvas gameCanvas;
        public GameObject endGamePanel;
        [Space]
        public Button pauseButton;
        public GameObject pauseMenu;

        [Header("About Song")]
        public Slider songTimeBar;
        public TMP_Text starsCounter;

        [Header("Combo and Score feedback")]
        public TMP_Text comboText;
        public TMP_Text scoreText;
        public Slider scoreBar;

        [Header("Skill feedback")]
        public Button skillButton;
        public Slider skillBarSlider;

        [Header("Song State")]
        public Timer songTimer;

        private int m_Combo, m_Score, m_StarsCount, m_Taps;
        private bool m_Started, m_Ended, m_IsPaused;

        private int m_ComboPrizeCounter, m_HighestCombo, m_NotesHit;

        private PlayerData m_Data;
        private GameManager m_GameManager;

        private float m_StartTime;
        //POWERS RELATED VARIABLES
        public bool CanMiss { get; set; } = true;

        private float m_Multiplier = 1f;
        public float Multiplier
        {
            get => m_Multiplier;
            set => m_Multiplier = value <= 0 ? 1f : value;
        }
        //------------------------

        protected override void Awake()
        {
            base.Awake();
            m_StartTime = Time.realtimeSinceStartup;
        }

        /// <summary>
        /// Sets the gameplay values
        /// </summary>
        private void Start()
        {
            m_Data = DataManager.Instance.playerData;
            m_GameManager = GameManager.Instance;

            scoreBar.minValue = songTimeBar.minValue = skillBarSlider.minValue = 0;

            SetPauseButton();

            SetCharacterSkills();

            ResetValues();

            StartMap();
        }

        /// <summary>
        /// Sets the Pause Button Click Listener
        /// </summary>
        private void SetPauseButton() =>
            pauseButton.onClick.AddListener(() =>
            {
                m_IsPaused = !m_IsPaused;
                if (m_IsPaused) PauseMap();
                else ResumeMap();
            });

        /// <summary>
        /// Activates the passive and sets the active skill
        /// </summary>
        private void SetCharacterSkills()
        {
            ScriptableCharacter character = GameManager.GetCharacter();

            //Use passive from the start
            character.passiveSkill.UseSkill();

            ScriptableSkill playerSkill = character.activeSkill;

            skillBarSlider.maxValue = playerSkill.rechargeQuantity;

            skillButton.onClick.AddListener(() =>
            {
                if (skillBarSlider.value >= skillBarSlider.maxValue)
                {
                    //Use active only when available
                    playerSkill.UseSkill();
                    skillBarSlider.value = 0;
                }
            });
        }

        /// <summary>
        /// Resets all the values to 0, for a replay
        /// </summary>
        private void ResetValues()
        {
            //Reset slider and private values to 0
            m_Ended = false;

            m_Combo = m_Score = m_StarsCount = m_Taps = m_ComboPrizeCounter = m_HighestCombo = m_NotesHit = 0;
            scoreBar.value = songTimeBar.value = skillBarSlider.value = 0;

            starsCounter.text = "0";
            comboText.text = "x0";
            scoreText.text = "0";
        }

        /// <summary>
        /// Saves the time played on the scene and the taps done, if not ended
        /// </summary>
        private void OnDestroy()
        {
            //Save the time played
            m_Data.timePlayedInGame += Time.realtimeSinceStartup - m_StartTime;
            //If it didn't see the final screen then save the tapsDone
            if (!m_Ended) m_Data.tapsDone += m_Taps;
        }

        /// <summary>
        /// Updates the song time ui
        /// </summary>
        private void Update()
        {
            if (!m_Started) return;
            songTimeBar.value = songTimer.CurrentTime;
        }

        /// <summary>
        /// Starts the whole gameplay
        /// </summary>
        public void StartMap()
        {
            m_Ended = false;
            m_Started = true;

            SoundMap soundMap = GameManager.GetSoundMap();

            SetSongValues(soundMap.audioClip.length, soundMap.notes.Length);
            songTimer.StartTimer();

            mapScroller.SetSoundMap(soundMap, true);
            mapScroller.StartMap();
        }

        /// <summary>
        /// Sets the initial songs values for the ui
        /// </summary>
        /// <param name="time"></param>
        /// <param name="notes"></param>
        private void SetSongValues(float time, int notes)
        {
            songTimer.SetTimer(new TimerOptions(time, TimerType.Progressive, false));
            songTimer.onTimerStop += StopMap;

            songTimeBar.maxValue = time;
            scoreBar.maxValue = notes < 1 ? 0 : Mathf.Round(notes.FactorialSum() / 7f);
        }

        /// <summary>
        /// Pauses gameplay and song timer but not time
        /// </summary>
        public void PauseMap()
        {
            m_Started = false;
            songTimer.PauseTimer();
            mapScroller.StopMap();
            pauseMenu.SetActive(true);
        }

        /// <summary>
        /// Resumes gameplay and song timer
        /// </summary>
        public void ResumeMap()
        {
            m_Started = true;
            songTimer.UnpauseTimer();
            mapScroller.ResumeMap();
            pauseMenu.SetActive(false);
            m_IsPaused = false;
        }

        /// <summary>
        /// Ends the gameplay, and shows the end screen
        /// </summary>
        public void StopMap()
        {
            m_Ended = true;

            m_Started = false;
            songTimer.onTimerStop -= StopMap;

            ShowEndGameplayPanel(gameCanvas);
        }

        /// <summary>
        /// Shows the end gameplay panel
        /// </summary>
        /// <param name="parentCanvas"></param>
        public void ShowEndGameplayPanel(Canvas parentCanvas)
        {
            const float moneyMultiplier = 25 * 0.01f;
            float accuracy = m_NotesHit * 100f / mapScroller.MapNotesQuantity;

            var accuracyGain = (int)Mathf.Round(accuracy * moneyMultiplier);
            var comboGain = (int)Mathf.Round(m_HighestCombo * moneyMultiplier);

            m_Data.money += accuracyGain + comboGain;
            m_Data.tapsDone += m_Taps;

            ScriptableCharacter character = GameManager.GetCharacter();
            SoundMap soundMap = mapScroller.SoundMap;

            var gameOverPanel = Instantiate(endGamePanel, parentCanvas.transform).GetComponent<GameOverPanel>();
            //Set end panel values
            gameOverPanel.SetSongName(soundMap.name);
            gameOverPanel.SetCharacterVisuals(character);
            gameOverPanel.SetCharacterBonus(character.characterGenre, soundMap.genre);
            
            gameOverPanel.SetScore(m_Score);
            gameOverPanel.SetStars(m_StarsCount, character);
            gameOverPanel.SetAccuracy(soundMap.notes.Length, m_NotesHit, accuracy);
            
            gameOverPanel.SetMapMaker(soundMap.mapCreator);
            gameOverPanel.SetGroupName(soundMap.genre);
            gameOverPanel.SetHighestCombo(m_HighestCombo);
                
            gameOverPanel.SetNewHighScoreText(0, m_Score);
            
            gameOverPanel.replaySongButton.onClick.AddListener(LevelLoadManager.LoadArrowGameplayScene);
            //Give prizes
            //Get panel stars or whatever
        }

        /// <summary>
        /// Logic for missing an arrow without tapping
        /// </summary>
        public static void MissArrow()
        {
            // Can miss check in case of power
            if (!m_Instance || !m_Instance.CanMiss) return;

            //Save highest combo
            int combo = m_Instance.m_Combo;
            if (combo > m_Instance.m_HighestCombo) m_Instance.m_HighestCombo = combo;

            //Reset combo to 0
            m_Instance.comboText.text = $"x{m_Instance.m_Combo = 0}";
        }

        /// <summary>
        /// Logic for missing an arrow by tapping
        /// </summary>
        public static void MissArrowTap()
        {
            if (!m_Instance) return;

            m_Instance.m_Taps++;
            MissArrow();
        }

        /// <summary>
        /// Logic for hitting an arrow correctly
        /// </summary>
        /// <param name="isCombo"></param>
        /// <param name="comboLength"></param>
        public static void HitArrow(bool isCombo = false, int comboLength = 0)
        {
            if (!m_Instance) return;

            const int ARROW_HIT_VALUE = 1;

            //Increment combo, taps, notes hit for player stats
            m_Instance.m_Taps++;
            m_Instance.m_NotesHit++;
            m_Instance.skillBarSlider.value++;

            //Get the points multiply by the combo and multiplier and finally rounded
            int points = Mathf.RoundToInt(ARROW_HIT_VALUE * ++m_Instance.m_Combo * m_Instance.Multiplier);

            m_Instance.scoreText.text = $"{m_Instance.m_Score += points}";
            m_Instance.comboText.text = $"x{m_Instance.m_Combo}";

            Slider bar = m_Instance.scoreBar;

            bar.value = (bar.value + points) % bar.maxValue;

            //Increment the start count if went upper the score limit
            if (bar.value >= bar.maxValue)
            {
                bar.value = 0;
                m_Instance.starsCounter.text = $"{++m_Instance.m_StarsCount}";
            }

            //Check if is combo or if is in middle of a combo and give prize
            if (isCombo && ++m_Instance.m_ComboPrizeCounter >= comboLength)
            {
                Song song = m_Instance.m_GameManager.Song;

                const int maxMoneyGain = 5 + 1, minMoneyGain = 3;

                m_Instance.skillBarSlider.value += maxMoneyGain + minMoneyGain;

                if (song) m_Instance.m_Data.money += Random.Range(minMoneyGain, maxMoneyGain);
            }
            else
                m_Instance.m_ComboPrizeCounter = 0;
        }
    }
}
