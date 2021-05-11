using TMPro;
using UnityEngine;

namespace Plugins.Tools
{
    public enum ShowType { NoFormat, ClockLike, IntegerOnly }

    public class TimerUI : Timer
    {
        [Header("UI")]
        public TMP_Text timerText;
        public ShowType showType = ShowType.NoFormat;

        protected override void Update()
        {
            base.Update();
            if (!IsTimerStarted) return;
            UpdateTimerText();
        }

        private void UpdateTimerText()
        {
            string text = showType switch
            {
                ShowType.ClockLike => $"{Mathf.Floor(m_Timer / 60) % 60:00}:{m_Timer % 60:00}",
                ShowType.NoFormat => m_Timer + "",
                ShowType.IntegerOnly => $"{Mathf.Floor(m_Timer)}",
                _ => ""
            };
            timerText.text = text;
        }
    }
}
