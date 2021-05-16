using General;
using Managers;
using Plugins.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SongRecordScreen : MonoBehaviour
    {
        public Image characterImage;
        public TMP_Text scoreText, gradeText, comboText, accuracyText;
        public Button playButton;

        private Song m_Song;

        private void Start() => playButton.onClick.AddListener(() =>
        {
            GameManager.PutSoundMap(m_Song);
            LevelLoadManager.LoadArrowGameplayScene();
        });

        private void OnDisable() => SoundManager.Instance.StopBackgroundMusic();

        public void ShowRecordScreen(Sprite sprite, int score, string grade, int combo, float accuracy)
        {
            characterImage.sprite = sprite;
            scoreText.text = $"Score: {score}";
            comboText.text = $"Highest Combo: {combo}";
            accuracyText.text = $"{accuracy}%";
            gradeText.text = grade;
        }

        public void ShowRecordScreen(Song song)
        {
            gameObject.SetActive(true);
            
            SoundManager.Instance.PlayBackgroundMusicNoFade(song.soundMap.audioClip);
            
            //TODO: serialize this song values
            ShowRecordScreen(song.songImage, 100, "SSS", 50, 75);
            
            m_Song = song;
        }
    }
}
