using Core.Arrow_Game;
using General;
using Plugins.GUI;
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
        public SelectableButton pauseSelectableButton;

        [Header("About Song")]
        public Slider songTimeBar;
        public TMP_Text starsCounter;

        [Header("Combo and Score feedback")]
        public TMP_Text comboText;
        public TMP_Text scoreText;
        public Slider comboBar, scoreBar;

        [Header("Skill feedback")]
        public SelectableButton skillSelectableButton;
        public Slider skillBarSlider;

        private Timer songTimer;
        private void Start()
        {
            songTimer = Timer.CreateTimerInstance(gameObject);
            StartMap();
        }

        public void StartMap()
        {
            SoundMap soundMap = GameManager.GetSoundMap();

            songTimer.SetTimer(new TimerOptions(soundMap.audioClip.length, TimerType.Progressive, false));
            songTimer.events.onTimerStop.AddListener(StopMap);
            songTimer.StartTimer();

            mapScroller.SetSoundMap(soundMap);
            mapScroller.StartMap();
        }

        public void PauseMap()
        {
            songTimer.PauseTimer();
            mapScroller.StopMap();
        }

        public void ResumeMap()
        {
            songTimer.UnpauseTimer();
            mapScroller.ResumeMap();
        }

        public void StopMap()
        {
            songTimer.events.onTimerStop.RemoveListener(StopMap);
            ShowEndGameplayPanel(gameCanvas);
        }

        public void ShowEndGameplayPanel(Canvas parentCanvas)
        {
            Instantiate(endGamePanel, parentCanvas.transform);
            //Give prizes
            //Get panel stars or whatever
        }

        public static void MissArrow() => DataManager.playerData.tapsDone++;

        public static void HitArrow()
        {
            DataManager.playerData.tapsDone++;

            Song song = GameManager.Instance.Song;

            if (song)
            {
                //Check for probability of gain money/prize    
            }
        }
    }
}
