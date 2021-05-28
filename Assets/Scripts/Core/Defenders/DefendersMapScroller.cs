using UnityEngine;
using Plugins.Audio;
using Utilities;

namespace Core.Defenders
{
    public class DefendersMapScroller : MonoBehaviour
    {
        public float bps;
        private bool m_IsStarted;
        public Direction direction;

        private void Update()
        {
            if (!m_IsStarted) return;
            DirectionMovement();
        }

        private void DirectionMovement()
        {
            switch (direction)
            {
                case Direction.West:
                    transform.position -= new Vector3(bps * SoundManager.songDeltaTime, 0f, 0f);
                    break;
                case Direction.East:
                    transform.position += new Vector3(bps * SoundManager.songDeltaTime, 0f, 0f);
                    break;
                case Direction.North:
                    transform.position += new Vector3(0f, bps * SoundManager.songDeltaTime, 0f);
                    break;
                case Direction.South:
                    transform.position -= new Vector3(0f, bps * SoundManager.songDeltaTime, 0f);
                    break;
            }
        }
        public void StartMap(float beatPerSec)
        {
            m_IsStarted = true;
            bps = beatPerSec;
        }

        public void StopMap() => m_IsStarted = false;
    }
}
