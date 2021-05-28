using Plugins.Tools;
using UnityEngine;
using TMPro;
using ScriptableObjects;
using UI;
using UnityEngine.UI;

namespace Managers
{
    public enum Genre { Kpop, Metal, Edm, VideoGames, Rap, Custom }

    public class GameOverPanel : MonoBehaviour
    {
        public TMP_Text songName,
                        groupName,
                        mapMaker,
                        score,
                        newHighScoreText,
                        characterBonus,
                        difficulty,
                        scoreInMiddlePart,
                        accuracy,
                        highestCombo,
                        backToMenuButtonText,
                        replayButton,
                        randomSongButtonText,
                        comboRewardText,
                        accuracyRewardText;

        [Space]
        public Image[] stars;
        public GUIImage[] buttons;
        public Image panel;

        public Button replaySongButton, randomSong;

        public void SetStars(int starsNumber, Sprite fullStar, Sprite emptyStar)
        {
            starsNumber = Mathf.Min(stars.Length, starsNumber);
            int i;

            for (i = 0; i < starsNumber; i++) stars[i].sprite = fullStar;

            if (starsNumber >= stars.Length) return;

            for (int k = i; k < stars.Length; k++) stars[k].sprite = emptyStar;
        }

        public void SetCharacterVisuals(ScriptableCharacter character)
        {
            accuracyRewardText.font = comboRewardText.font = songName.font
                = groupName.font = mapMaker.font = score.font = newHighScoreText.font = characterBonus.font
                    = difficulty.font = scoreInMiddlePart.font = accuracy.font = highestCombo.font
                        = backToMenuButtonText.font = replayButton.font = randomSongButtonText.font = character.font;

            panel.sprite = character.backgroundImage;

            Palette palette = character.colorPalette;
            buttons.ForEach(image => image.SetColor(palette));
        }

        public void SetScore(int oldScore, int newScore)
        {
            string scoreString = newScore + "";
            
            score.text = scoreString;
            scoreInMiddlePart.text = $"Score: {scoreString}";
            
            newHighScoreText.gameObject.SetActive(oldScore < newScore);
        }

        public void SetSongName(string nameOfTheSong) => songName.text = nameOfTheSong;

        public void SetGroupName(Genre genre) => groupName.text = $"Song Genre: {genre}";

        public void SetMapMaker(string mapCreator) => mapMaker.text = mapCreator;

        public void SetCharacterBonus(Genre charGenre, Genre genreOfTheSong)
        {
            string bonusText = charGenre == genreOfTheSong ? "x1.2" : "x1";
            characterBonus.text = $"Character Bonus: ({charGenre} {bonusText} multiplier)";
        }

        public void SetAccuracy(int totalBeats, int hitBeats, float acc) => accuracy.text = $"Accuracy: {hitBeats} / {totalBeats} ({acc:N2}%)";

        public void SetHighestCombo(int combo) => highestCombo.text = "Highest Combo: " + combo;

        public void SetRewardsText(int comboGain, int accuracyGain)
        {
            comboRewardText.text = $"+{comboGain} Combo coins";
            accuracyRewardText.text = $"+{accuracyGain} Accuracy coins";
        }
    }
}
