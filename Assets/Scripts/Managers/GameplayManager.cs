using System;
using System.Collections;
using Core.Arrow_Game;
using General;
using Plugins.Audio;
using Plugins.Tools;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using Random = UnityEngine.Random;

namespace Managers
{
    public enum HitType { Good = 3, Perfect = 5, TooSlow = 2, TooSoon = 2 }

    public class GameplayManager : Singleton<GameplayManager>
    {
        public MapScroller mapScroller;

        [Header("Config")]
        public Character currentCharacter;
        public SimpleFeedbackObjectPooler feedbackTextPooler, skillGainTextPooler;
        [Space]
        public Transform gameCanvas;
        public GameObject endGamePanel;
        [Space]
        public Button pauseButton;
        public GameObject pauseMenu;

        [Header("About Song")]
        public Slider songTimeBar;
        public TMP_Text starsCounter;

        [Header("Combo and Score feedback")]
        public Slider scoreBar;
        public TMP_Text comboText, scoreText;

        [Header("Skill feedback")]
        public Button skillButton;
        public Slider skillBarSlider;

        [Header("Song State")]
        public Timer songTimer;

        private int m_Combo, m_Score, m_StarsCount, m_Taps;
        private bool m_Started, m_Ended, m_IsPaused;

        private int m_ComboPrizeCounter, m_HighestCombo, m_NotesHit;

        private PlayerData m_Data;

        private float m_StartTime;
        private Vector3 m_SkillSliderPosition;

        public bool CanLose { get; set; } = true;
        
        private const int maxMoneyGain = 5 + 1, minMoneyGain = 3;

        //POWERS RELATED VARIABLES
        public bool CanMiss { get; set; } = true;
        public float Multiplier
        {
            get => m_Multiplier;
            set => m_Multiplier = value < 1f ? 1f : value;
        }
        private float m_Multiplier = 1f;
        public float DurationIncrement { get; set; }

        public float MoneyMultiplier { get; set; } = .25f;
        public int MinimumDamage { get; set; } = 9;
        public bool EveryNoteGivesMoney { get; set; }
        public bool ComboTimeHeal { get; set; }
        public float HealingValueComboTime { get; set; }
        public int MinimumCombo { get; set; }

        public int FreeTapsCount { get; set; }

        private int m_MissedTaps;

        //------------------------

        protected override void Awake()
        {
            base.Awake();
            m_StartTime = Time.realtimeSinceStartup;
        }

        public void SetFreeTaps(int taps) => FreeTapsCount = m_MissedTaps + taps;

        /// <summary>
        /// Sets the gameplay values
        /// </summary>
        private void Start()
        {
            m_Data = DataManager.Instance.playerData;

            scoreBar.minValue = songTimeBar.minValue = skillBarSlider.minValue = 0;

            m_SkillSliderPosition = skillBarSlider.transform.position;

            SetPauseButton();

            SetGameplayCharacter();

            ResetValues();

            ScriptableRune rune = GameManager.GetRune();
            if (rune) rune.ActivateRune(this);

            CanMiss = false;
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
        private void SetGameplayCharacter()
        {
            currentCharacter.onDie += Lose;

            currentCharacter.SetCharacter(GameManager.GetCharacter(), this);

            skillBarSlider.maxValue = currentCharacter.character.activeSkill.rechargeQuantity;

            skillButton.onClick.AddListener(() =>
            {
                if (skillBarSlider.value >= skillBarSlider.maxValue)
                {
                    //Use active only when available
                    currentCharacter.UsePower(this);
                    skillBarSlider.value = 0;
                }
            });
        }

        /// <summary>
        /// Loses the game and calls everything related
        /// </summary>
        public void Lose() =>
            SlowTime(2f, 0f, () =>
            {
                mapScroller.StopMap();
                ShowEndGameplayPanel(gameCanvas);
                SoundManager.Instance.backgroundAudioSource.Stop();
            });

        /// <summary>
        /// Slows the game time
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="objective"></param>
        /// <param name="afterSlowingTime"></param>
        public void SlowTime(float duration, float objective, Action afterSlowingTime = null) => StartCoroutine(SlowTimeCoroutine(duration, objective, afterSlowingTime));

        /// <summary>
        /// Slow time coroutine
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="objective"></param>
        /// <param name="afterSlowingTime"></param>
        /// <returns></returns>
        public static IEnumerator SlowTimeCoroutine(float duration, float objective, Action afterSlowingTime)
        {
            AudioSource sound = SoundManager.Instance.backgroundAudioSource;
            
            var currentTime = 0f;
            float startValue = Time.timeScale;
            while (duration > currentTime)
            {
                sound.pitch = Time.timeScale = Mathf.Lerp(startValue, objective, currentTime / duration);
                currentTime += Time.unscaledDeltaTime;
                yield return null;
            }

            sound.pitch = Time.timeScale = 1f;

            afterSlowingTime?.Invoke();
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
            m_Started = CanMiss = true;

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
            time = Mathf.Ceil(time) + 1f; //We add extra length to assure ending

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
        private void ShowEndGameplayPanel(Transform parentCanvas)
        {
            CheckHighestCombo();

            float accuracy = m_NotesHit * 100f / mapScroller.MapNotesQuantity;

            var accuracyGain = (int)Mathf.Round(accuracy * MoneyMultiplier);
            var comboGain = (int)Mathf.Round(m_HighestCombo * MoneyMultiplier);

            m_Data.money += accuracyGain + comboGain;
            m_Data.tapsDone += m_Taps;

            SoundMap soundMap = mapScroller.SoundMap;
            ScriptableCharacter character = GameManager.GetCharacter();

            SerializableSong songData = DataManager.GetSong(soundMap.ID);
            if (songData.songId == 0) songData.SetId(soundMap.ID);

            int oldHighScore = songData.highestScore;

            var gameOverPanel = Instantiate(endGamePanel, parentCanvas).GetComponent<GameOverPanel>();
            //Set end panel values
            gameOverPanel.SetSongName(soundMap.name);
            gameOverPanel.SetCharacterVisuals(character);
            gameOverPanel.SetCharacterBonus(character.characterGenre, soundMap.genre);

            gameOverPanel.SetScore(songData.highestScore, m_Score);
            gameOverPanel.SetStars(m_StarsCount, character);
            gameOverPanel.SetAccuracy(soundMap.notes.Length, m_NotesHit, accuracy);

            gameOverPanel.SetMapMaker(soundMap.mapCreator);
            gameOverPanel.SetGroupName(soundMap.genre);

            gameOverPanel.SetHighestCombo(m_HighestCombo);
            gameOverPanel.SetRewardsText(comboGain, accuracyGain);

            gameOverPanel.replaySongButton.onClick.AddListener(LevelLoadManager.LoadArrowGameplayScene);

            //Show prizes

            if (m_Score <= oldHighScore) return;

            songData.UpdateValues(m_Combo, m_Score, accuracy);
            DataManager.UpdateSong(songData);
        }

        /// <summary>
        /// Logic for missing an arrow without tapping
        /// </summary>
        public static void MissArrow()
        {
            // Can miss check in case of power
            if (!m_Instance || !m_Instance.CanMiss) return;

            if (m_Instance.m_Combo > m_Instance.MinimumCombo)
            {
                CheckHighestCombo();
                //Reset combo to 0
                m_Instance.comboText.text = $"x{m_Instance.m_Combo = 0}";
            }

            if (m_Instance.FreeTapsCount > ++m_Instance.m_MissedTaps) return;

            Character character = m_Instance.currentCharacter;

            if (m_Instance.CanLose && !character.IsDead) character.TakeDamage(m_Instance.MinimumDamage);
        }

        /// <summary>
        /// Saves highest combo
        /// </summary>
        private static void CheckHighestCombo()
        {
            int combo = m_Instance.m_Combo;
            if (combo > m_Instance.m_HighestCombo) m_Instance.m_HighestCombo = combo;
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
        /// <param name="hitType"></param>
        /// <param name="feedbackPosition"></param>
        /// <param name="feedbackColor"></param>
        /// <param name="isCombo"></param>
        /// <param name="comboLength"></param>
        public static void HitArrow(HitType hitType, Vector3 feedbackPosition, Color feedbackColor, bool isCombo = false, int comboLength = 0)
        {
            if (!m_Instance) return;

            //Increment combo, taps, notes hit for player stats
            m_Instance.m_Taps++;
            m_Instance.m_NotesHit++;
            m_Instance.skillBarSlider.value++;

            m_Instance.skillGainTextPooler.ShowText($"+1", m_Instance.m_SkillSliderPosition);

            //Get the points multiply by the combo and multiplier and finally rounded
            int points = Mathf.RoundToInt((int)hitType * ++m_Instance.m_Combo * m_Instance.Multiplier);

            m_Instance.scoreText.text = $"{m_Instance.m_Score += points}";
            m_Instance.comboText.text = $"x{m_Instance.m_Combo}";

            Slider bar = m_Instance.scoreBar;

            float newScoreValue = bar.value + points;
            bar.value = newScoreValue % bar.maxValue;

            //Increment the start count if went upper the score limit
            if (newScoreValue >= bar.maxValue)
            {
                m_Instance.starsCounter.text = $"{++m_Instance.m_StarsCount}";
            }

            if (m_Instance.EveryNoteGivesMoney) GiveMoney();

            //Check if is combo or if is in middle of a combo and give prize
            if (isCombo)
            {
                if (m_Instance.ComboTimeHeal) m_Instance.currentCharacter.Heal(m_Instance.HealingValueComboTime);
                //TODO: Responsive money gain, damage done and skill gain

                bool completedCombo = ++m_Instance.m_ComboPrizeCounter >= comboLength;
                m_Instance.skillBarSlider.value += completedCombo ? (maxMoneyGain + minMoneyGain) * comboLength : maxMoneyGain + minMoneyGain;

                m_Instance.skillGainTextPooler.ShowText($"+{maxMoneyGain + minMoneyGain}", m_Instance.m_SkillSliderPosition);

                GiveMoney();
            }
            else
                m_Instance.m_ComboPrizeCounter = 0;

            //Hit Feedback
            m_Instance.feedbackTextPooler.ShowText(hitType.ToString(), feedbackColor, feedbackPosition);
        }

        private static void GiveMoney() => m_Instance.m_Data.money += Random.Range(minMoneyGain, maxMoneyGain);
    }
}
