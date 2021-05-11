using Core.Arrow_Game;
using General;
using Plugins.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

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

        private Timer m_SongTimer;
        private int m_Combo, m_Score, m_StarsCount, m_Taps;
        private bool m_Started;

        protected override void Awake()
        {
            base.Awake();
            m_SongTimer = Timer.CreateTimerInstance(gameObject);
        }

        private void Start()
        {
            scoreBar.minValue = 0;
            songTimeBar.minValue = 0;
            skillBarSlider.minValue = 0;

            starsCounter.text = "0";
            comboText.text = "x0";
            scoreText.text = "0";

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

            SetTimer(soundMap.audioClip.length);
            m_SongTimer.StartTimer();

            mapScroller.SetSoundMap(soundMap);
            mapScroller.StartMap();
        }

        private void SetTimer(float time)
        {
            m_SongTimer.SetTimer(new TimerOptions(time, TimerType.Progressive, false));
            m_SongTimer.onTimerStop += StopMap;

            songTimeBar.maxValue = time;
            scoreBar.maxValue = time; //TODO: set correct max value for m_Score bar and skillbar
            skillBarSlider.maxValue = time;
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
            Instantiate(endGamePanel, parentCanvas.transform);
            //Give prizes
            //Get panel stars or whatever
        }

        public static void MissArrow() => m_Instance.comboText.text = $"x{m_Instance.m_Combo = 0}";

        public static void MissArrowTap()
        {
            m_Instance.m_Taps++;
            MissArrow();
        }

        public static void HitArrow()
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

            Song song = GameManager.Instance.Song;

            if (song)
            {
                //Check for probability of gain money/prize    
            }
        }
    }
}
