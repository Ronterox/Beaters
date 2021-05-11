using System;
using Core.Arrow_Game;
using General;
using Plugins.Tools;
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

        [Header("Start Feedback")]
        public TimerUI startTimer;

        private Timer m_SongTimer;
        private int m_Combo, m_Score, m_StarsCount, m_Taps;
        private bool m_Started;

        private int m_ComboPrizeCounter, m_HighestCombo;

        protected override void Awake()
        {
            base.Awake();
            m_SongTimer = Timer.CreateTimerInstance(gameObject);
        }

        private void Start()
        {
            scoreBar.minValue = songTimeBar.minValue = skillBarSlider.minValue = 0;

            ResetValues();

            startTimer.onTimerStop += () =>
            {
                startTimer.timerText.text = "Go!";
                Action deactivate = () => startTimer.gameObject.SetActive(false);
                deactivate.DelayAction(1f);
            };

            StartGame();
        }
        
        private void ResetValues()
        {
            m_Combo = m_Score = m_StarsCount = m_Taps  = m_ComboPrizeCounter = m_HighestCombo = 0;
            
            starsCounter.text = "0";
            comboText.text = "x0";
            scoreText.text = "0";
        }

        public void StartGame()
        {
            startTimer.timerText.text = "Ready?";
            startTimer.gameObject.SetActive(true);

            Action activateTimer = () => startTimer.StartTimer();
            activateTimer.DelayAction(1f);

            StartMap();
        }

        private void Update()
        {
            if (!m_Started) return;
            songTimeBar.value = m_SongTimer.CurrentTime;
        }

        public void StartMap()
        {
            m_Started = true;

            SoundMap soundMap = GameManager.GetSoundMap();

            SetSongValues(soundMap.audioClip.length, soundMap.notes.Length);
            m_SongTimer.StartTimer();

            mapScroller.SetSoundMap(soundMap, true);
            mapScroller.StartMap();
        }

        private void SetSongValues(float time, int notes)
        {
            m_SongTimer.SetTimer(new TimerOptions(time, TimerType.Progressive, false));
            m_SongTimer.onTimerStop += StopMap;

            songTimeBar.maxValue = time;
            scoreBar.maxValue = notes < 1 ? 0 : Mathf.Round(notes.FactorialSum() / 7f);
            skillBarSlider.maxValue = time; //TODO: set max value by character skill
        }

        public void PauseMap()
        {
            m_Started = false;
            m_SongTimer.PauseTimer();
            mapScroller.StopMap();
        }

        public void ResumeMap()
        {
            m_Started = true;
            m_SongTimer.UnpauseTimer();
            mapScroller.ResumeMap();
        }

        public void StopMap()
        {
            m_Started = false;
            m_SongTimer.onTimerStop -= StopMap;
            ShowEndGameplayPanel(gameCanvas);
            print(m_Taps);
        }

        public void ShowEndGameplayPanel(Canvas parentCanvas)
        {
            const float moneyMultiplier = 25 * 0.01f;
            float accuracy = mapScroller.MapNotesQuantity * 100f / m_Instance.m_Taps;
            
            var accuracyGain = (int)Mathf.Round(accuracy * moneyMultiplier);
            var comboGain = (int)Mathf.Round(m_HighestCombo * moneyMultiplier);

            DataManager.playerData.money += accuracyGain + comboGain;

            Instantiate(endGamePanel, parentCanvas.transform);
            //Give prizes
            //Get panel stars or whatever
        }

        public static void MissArrow()
        {
            int combo = m_Instance.m_Combo;
            if (combo > m_Instance.m_HighestCombo) m_Instance.m_HighestCombo = combo;
            
            m_Instance.comboText.text = $"x{m_Instance.m_Combo = 0}";
        }

        public static void MissArrowTap()
        {
            m_Instance.m_Taps++;
            MissArrow();
        }

        public static void HitArrow(bool isCombo = false, int comboLength = 0)
        {
            const int ARROW_HIT_VALUE = 1;

            m_Instance.m_Taps++;

            int points = ARROW_HIT_VALUE * ++m_Instance.m_Combo;

            m_Instance.scoreText.text = $"{m_Instance.m_Score += points}";
            m_Instance.comboText.text = $"x{m_Instance.m_Combo}";

            Slider bar = m_Instance.scoreBar;
            bar.value += points;

            if (bar.value >= bar.maxValue)
            {
                bar.value = 0;
                m_Instance.starsCounter.text = $"{++m_Instance.m_StarsCount}";
            }

            if (isCombo && ++m_Instance.m_ComboPrizeCounter >= comboLength)
            {
                Song song = GameManager.Instance.Song;

                const int maxMoneyGain = 5 + 1, minMoneyGain = 3;

                if (song)
                {
                    DataManager.playerData.money += Random.Range(minMoneyGain, maxMoneyGain);
                }
            }
            else
            {
                m_Instance.m_ComboPrizeCounter = 0;
            }
        }
    }
}