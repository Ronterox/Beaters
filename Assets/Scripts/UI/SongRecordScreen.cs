using Managers;
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

        private void Start() => playButton.onClick.AddListener(LevelLoadManager.LoadArrowGameplayScene);

        public void ShowRecordScreen(Sprite sprite, int score, string grade, int combo, float accuracy)
        {
            characterImage.sprite = sprite;
            scoreText.text = $"Score: {score}";
            comboText.text = $"Highest Combo: {combo}";
            accuracyText.text = $"{accuracy}%";
            gradeText.text = grade;
        }
    }
}
