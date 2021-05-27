using System;
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
            
            skillBarSlider.gameObject.SetActive(false);
            feedbackImage.gameObject.SetActive(false);

            SetPauseButton();

            ResetValues();

            CanMiss = false;
        }

        /*protected override void ShowEndGameplayPanel(Transform parentCanvas, bool win)
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

            gameOverPanel.gameObject.SetActive(false);

            Action activateEndScreen = () => gameOverPanel.gameObject.SetActive(true);

            finishText.text = win ? "You WIN!" : "You LOSE!";
            finishText.gameObject.SetActive(true);

            activateEndScreen.DelayAction(2);

            //Show prizes

            if (m_Score <= oldHighScore) return;

            songData.UpdateValues(m_Combo, m_Score, accuracy);
            DataManager.UpdateSong(songData);
        }*/
    }
}
