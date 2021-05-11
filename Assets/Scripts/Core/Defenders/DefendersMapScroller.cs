using UnityEngine;
using Plugins.Audio;

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
                case Direction.left:
                    transform.position -= new Vector3(bps * SoundManager.songDeltaTime, 0f, 0f);
                    break;
                case Direction.right:
                    transform.position += new Vector3(bps * SoundManager.songDeltaTime, 0f, 0f);
                    break;
                case Direction.up:
                    transform.position += new Vector3(0f, bps * SoundManager.songDeltaTime, 0f);
                    break;
                case Direction.down:
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
