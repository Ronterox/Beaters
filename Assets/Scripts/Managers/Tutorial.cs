using System;
using General;
using Plugins.Tools;
using ScriptableObjects;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace Managers
{
    public class Tutorial : GameplayManager
    {
        public Song tutorialSong;
        protected override void Start()
        {
            GameManager.PutSoundMap(tutorialSong);

            m_Data = DataManager.Instance.playerData;

            m_SkillSliderPosition = skillBarSlider.transform.position;
            
            skillBarSlider.gameObject.SetActive(false);
            feedbackImage.gameObject.SetActive(false);

            ResetValues();
        }

        public override void StartMap()
        {
            base.StartMap();
            CanMiss = false;
        }

        protected override void ShowEndGameplayPanel(Transform parentCanvas, bool win)
        {
            CheckHighestCombo();

            float accuracy = m_NotesHit * 100f / mapScroller.MapNotesQuantity;

            float rng = Random.Range(m_MinMoneyGain, m_MaxMoneyGain);
            const float percentageGainLimiter = .10f;

            int accuracyGain = Mathf.RoundToInt((accuracy * percentageGainLimiter + rng) * MoneyMultiplier),
                comboGain = Mathf.RoundToInt((m_HighestCombo * percentageGainLimiter + rng) * MoneyMultiplier);

            m_Data.money += accuracyGain + comboGain;
            m_Data.tapsDone += m_Taps;

            SoundMap soundMap = mapScroller.SoundMap;

            SerializableSong songData = DataManager.GetSong(soundMap.ID) ?? new SerializableSong { songId = soundMap.ID };

            int oldHighScore = songData.highestScore;

            Action endTutorial = () =>
            {
                LevelLoadManager.LoadMainMenu();
                PlayerPrefs.SetInt(TutorialButtons.FIRST_TIME_KEY, 1);
                PlayerPrefs.Save();
            };

            finishText.text = "You WIN!";
            finishText.gameObject.SetActive(true);

            endTutorial.DelayAction(2);

            //Show prizes

            if (m_Score <= oldHighScore) return;

            songData.UpdateValues(m_Combo, m_Score, accuracy);
            DataManager.UpdateSong(songData);
        }
    }
}
