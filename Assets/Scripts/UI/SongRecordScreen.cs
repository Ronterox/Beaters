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
        public Image songRecordImage;
        [Space]
        public Image characterImage;
        public TMP_Text scoreText, gradeText, comboText, accuracyText;
        [Space]
        public Button playButton;
        private Song m_Song;

        private void Start() => playButton.onClick.AddListener(() =>
        {
            GameManager.PutSoundMap(m_Song);
            LevelLoadManager.LoadArrowGameplayScene();
        });
        
        public void ShowRecordScreen(Sprite sprite, int score, string grade, int combo, float accuracy)
        {
            characterImage.sprite = sprite;
            scoreText.text = $"Score: {score}";
            comboText.text = $"Highest Combo: {combo}";
            accuracyText.text = $"Accuracy: {accuracy}%";
            gradeText.text = $"Grade: {grade}";
        }

        public void ShowRecordScreen(Song song)
        {
            gameObject.SetActive(true);
            
            SoundManager.Instance.PlayBackgroundMusicNoFade(song.soundMap.audioClip);

            SerializableSong serializableSong = DataManager.GetSong(song.ID);
            
            ShowRecordScreen(song.songImage, serializableSong.highestScore, "SSS", serializableSong.highestCombo, serializableSong.accuracy);
            
            m_Song = song;

            songRecordImage.sprite = song.songImage;
        }
    }
}
