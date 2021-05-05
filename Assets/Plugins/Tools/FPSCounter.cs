using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;

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

        private int m_Frames;              // Frames drawn over the interval
        private float m_AccumulatedFrames; // FPS accumulated frames over the interval

        public bool seeFps, seeMemoryUsage;

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
            var style = new GUIStyle(UnityEngine.GUI.skin.GetStyle("Label"))
            {
                fontSize = fontSize,
                alignment = TextAnchor.LowerLeft,
                wordWrap = false
            };

            var labelStyle = new GUIStyle(UnityEngine.GUI.skin.GetStyle("Box"))
            {
                alignment = TextAnchor.UpperLeft,
                fontSize = fontSize
            };

            float height = style.lineHeight + 16 + fontSize;
            float width = 200 - m_SafeZone + fontSize * 2.5f;
            
            if (seeFps)
            {
                var frameBox = new Rect(Screen.width - (width + screenPosOffset.x), screenPosOffset.y, width, height);
                UnityEngine.GUI.Box(frameBox, $"FPS, Build v{Application.version}", labelStyle);
                UnityEngine.GUI.Label(frameBox, $"{m_Fps:F2}");
            }

            if (seeMemoryUsage)
            {
                style.fontSize = (int)(fontSize * .5f);
                var frameBox2 = new Rect(Screen.width - (width + screenPosOffset.x), height + 30 + screenPosOffset.y, width, height + 10);
                UnityEngine.GUI.Box(frameBox2, "Memory", labelStyle);
                UnityEngine.GUI.Label(frameBox2, $"TotalAllocatedMemory : {Profiler.GetTotalAllocatedMemoryLong() / 1048576}mb"
                                                 + $"\nTotalReservedMemory : {Profiler.GetTotalReservedMemoryLong() / 1048576}mb"
                                                 + $"\nTotalUnusedReservedMemory : {Profiler.GetTotalUnusedReservedMemoryLong() / 1048576}mb");

                var frameBox3 = new Rect(Screen.width - 150, 30 + height * 2, 300 - m_SafeZone, height);
                UnityEngine.GUI.Label(frameBox3, $"Room : {SceneManager.GetActiveScene().name}");
            }
        }
    }
}
