using TMPro;
using UnityEngine;

namespace Plugins.Tools
{

    public class TimerUI : Timer
    {
        [Header("UI")]
        public TMP_Text m_TimerText;

        protected override void Update()
        {
            base.Update();
            UpdateTimerText();
        }

        private void UpdateTimerText()
        {
            if (m_TimerText) m_TimerText.text = $"{Mathf.Floor(m_Timer / 60) % 60:00}:{m_Timer % 60:00}";
        }
    }
}
