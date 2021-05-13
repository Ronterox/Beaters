using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ScriptableObjects;
using UnityEngine.UI;

namespace Managers{
    public enum Genre{
        kpop, metal, edm, videogames, rap
    }
    public class GameOverScript : MonoBehaviour
    {
        public TMP_Text songName, groupName, mapMaker, score, newHighScoreText, characterBonus, difficulty, scoreInMiddlePart, accuracy, highestCombo, backToMenuButton, replayButton, randomSongButton;
        public Image[] stars;
        public Image panel;


        public void SetStars(int stars, ScriptableCharacter character){
            int i;
            for(i=0;i<stars;i++){
                this.stars[i].sprite = character.fullStar;
            }
            if(stars<7){
                for(int k = i;k<7;k++){
                    this.stars[k].sprite = character.emptyStar;
                }
            }
        }

        public void SetFont(ScriptableCharacter character){
            songName.font = groupName.font =mapMaker.font = score.font = newHighScoreText.font = characterBonus.font = difficulty.font = scoreInMiddlePart.font = accuracy.font = highestCombo.font = backToMenuButton.font = replayButton.font = randomSongButton.font = character.font;
        }
        public void SetScore(int score){
            this.score.text = score.ToString();
        }
        public void SetSongName(string nameOfTheSong){
            songName.text = nameOfTheSong;
        }
        public void SetGroupName(string groupName){
            this.groupName.text = groupName;
        }
        public void SetMapMaker(string mapMaker){
            this.mapMaker.text = mapMaker;
        }
        public void SetNewHighScoreText(int previusHighScore, int lastScore){
            newHighScoreText.gameObject.SetActive(previusHighScore<lastScore);  
        }
        public void SetCharacterBonus(Genre currentCharName, Genre genreOfTheSong){
            characterBonus.text = currentCharName.ToString();
            if(currentCharName == genreOfTheSong){
                characterBonus.text += " (x1.2 multiplier)";
            }
        }
        public void SetScoreInMiddlePart(int score){
            scoreInMiddlePart.text = score.ToString();
        }
        public void SetAccuracy(int totalBeats, int hitBeats, float accuracy){
            this.accuracy.text = $"{totalBeats} / {hitBeats} ({accuracy}%)";
        }
        public void SetHighestCombo(int highestCombo){
            this.highestCombo.text = highestCombo.ToString();
        }
        public void SetBackgroundImage(ScriptableCharacter character){
            panel.sprite = character.backgroundImage;
        }
    }
}
