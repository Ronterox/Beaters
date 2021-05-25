using Managers;
using Plugins.Audio;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SongRecordScreen : MonoBehaviour
    {
        public Image songRecordImage, characterImage;
        
        [Space]
        public TMP_Text scoreText;
        public TMP_Text gradeText, comboText, accuracyText, songTimeText, modeText;
        
        [Space]
        public Button playButton;
        private Song m_Song;

        private void Start() => playButton.onClick.AddListener(() =>
        {
            GameManager.PutSoundMap(m_Song);
            LevelLoadManager.LoadArrowGameplayScene();
        });
        
        public void ShowRecordScreen(Sprite sprite, int score, string grade, int combo, float accuracy, float time, TMP_FontAsset fontAsset, GameMode gameMode)
        {
            characterImage.sprite = sprite;
            scoreText.text = $"Highest Score: {score}";
            comboText.text = $"Highest Combo: {combo}";
            accuracyText.text = $"Accuracy: {accuracy}%";
            gradeText.text = $"Highest Grade: {grade}";
            songTimeText.text = $"Length: {Mathf.Floor(time * 0.016665f) % 60:00}:{time % 60:00}";
            modeText.text = $"Song Mode: {gameMode}";

            scoreText.font = comboText.font = accuracyText.font = gradeText.font = songTimeText.font = modeText.font = fontAsset;
        }

        public void ShowRecordScreen(Song song)
        {
            gameObject.SetActive(true);
            
            SoundManager.Instance.PlayBackgroundMusicNoFade(song.soundMap.audioClip);

            SerializableSong serializableSong = DataManager.GetSong(song.ID);

            ScriptableCharacter character = GameManager.GetCharacter();
            
            ShowRecordScreen(character.sprites[0], serializableSong.highestScore, 
                             "No Grade", 
                             serializableSong.highestCombo, 
                             serializableSong.accuracy, song.soundMap.audioClip.length, character.font, GameMode.Hero);
            
            m_Song = song;

            songRecordImage.sprite = song.songImage;
        }
    }
}
