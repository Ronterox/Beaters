using UnityEngine;
using TMPro;
using ScriptableObjects;
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
                        backToMenuButton,
                        replayButton,
                        randomSongButton;

        [Space]
        public Image[] stars;
        public Image panel;

        public Button replaySongButton, randomSong;

        public void SetStars(int starsNumber, ScriptableCharacter character)
        {
            int i;

            for (i = 0; i < starsNumber; i++) stars[i].sprite = character.fullStar;

            if (starsNumber >= stars.Length) return;

            for (int k = i; k < stars.Length; k++) stars[k].sprite = character.emptyStar;
        }

        public void SetCharacterVisuals(ScriptableCharacter character)
        {
            songName.font = groupName.font = mapMaker.font =
                score.font = newHighScoreText.font = characterBonus.font =
                    difficulty.font = scoreInMiddlePart.font = accuracy.font =
                        highestCombo.font = backToMenuButton.font = replayButton.font = randomSongButton.font = character.font;

            panel.sprite = character.backgroundImage;
        }

        public void SetScore(int value)
        {
            var scoreString = value.ToString();
            score.text = scoreInMiddlePart.text = "Score: " + scoreString;
        }

        public void SetSongName(string nameOfTheSong) => songName.text = nameOfTheSong;

        public void SetGroupName(Genre genre) => groupName.text = $"Song Genre: {genre}";

        public void SetMapMaker(string mapCreator) => mapMaker.text = mapCreator;

        public void SetNewHighScoreText(int previouslyHighScore, int lastScore)
        {
            newHighScoreText.text = previouslyHighScore + "";
            newHighScoreText.gameObject.SetActive(previouslyHighScore < lastScore);
        }

        public void SetCharacterBonus(Genre charGenre, Genre genreOfTheSong)
        {
            string bonusText = charGenre == genreOfTheSong ? "x1.2" : "x1";
            characterBonus.text = $"Character Bonus: ({charGenre} {bonusText} multiplier)";
        }

        public void SetAccuracy(int totalBeats, int hitBeats, float acc) => accuracy.text = $"{hitBeats} / {totalBeats} ({acc}%)";

        public void SetHighestCombo(int combo) => highestCombo.text = "Highest Combo: " + combo;
    }
}
