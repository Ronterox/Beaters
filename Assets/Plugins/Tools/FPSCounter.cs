using UnityEngine;

namespace Plugins.Tools
{
    [AddComponentMenu("Penguins Mafia/Tools/FPS Counter")]
    public class FPSCounter : MonoBehaviour
    {
        public float updateInterval = 0.5f;
        public Vector2 screenPosOffset = Vector2.zero;
        private float m_TimeLeft; // Left time for current interval

        public int fontSize = 24;
        private int m_SafeZone;
        
        private float m_Fps;
        
        private int m_Frames;        // Frames drawn over the interval
        private float m_AccumulatedFrames; // FPS accumulated frames over the interval


        /// <summary>
        /// Start
        /// </summary>
        public void Start() => m_SafeZone = (int)(Screen.width * 0.05f);

        /// <summary>
        /// Update
        /// </summary>
        private void Update()
        {
            m_TimeLeft -= Time.deltaTime;
            m_AccumulatedFrames += Time.timeScale / Time.deltaTime;
            ++m_Frames;

            // Interval ended - update GUI text and start new interval
            if (m_TimeLeft > 0) return;
            // display two fractional digits (f2 format)
            m_Fps = m_AccumulatedFrames / m_Frames;
            m_TimeLeft = updateInterval;
            m_AccumulatedFrames = 0f;
            m_Frames = 0;
        }

        /// <summary>
        /// On GUI
        /// </summary>
        private void OnGUI()
        {
            GUIStyle style = UnityEngine.GUI.skin.GetStyle("Label");
            style.fontSize = fontSize;
            style.alignment = TextAnchor.LowerLeft;
            style.wordWrap = false;

            GUIStyle labelStyle = UnityEngine.GUI.skin.GetStyle("Box");
            labelStyle.alignment = TextAnchor.UpperLeft;
            labelStyle.fontSize = fontSize;

            float height = style.lineHeight + 16 + fontSize;
            float width = 200 - m_SafeZone + fontSize * 2.5f;
            var frameBox = new Rect(Screen.width - ( width + screenPosOffset.x), screenPosOffset.y, width, height);
            UnityEngine.GUI.Box(frameBox, $"FPS, Build v{Application.version}", labelStyle);
            UnityEngine.GUI.Label(frameBox, $"{m_Fps:F2}");
        }
    }
}
