using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Plugins.Audio;

namespace Core.Defenders{
    public class DefendersMapScroller : MonoBehaviour
    {
        private float bps;
        private bool m_IsStarted = false;
        public Direction direction;
        public SoundMap soundMap;
        public int bpm;

        private void Start() {
            m_IsStarted = true;
        }
        private void Update()
            {
                if (!m_IsStarted) return;
                DirectionMovement();
            }
            
        private void DirectionMovement(){
            switch(direction){
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
        private void Awake()
        {
            bps = bpm / 60 * (float)soundMap.difficulty;
        }
        public void SetSoundmap(SoundMap soundMap){
            
        }
    }
}