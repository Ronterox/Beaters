using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Plugins.Audio;

namespace Core.Defenders
{
    public class DefendersMapScrollerGeneral : MonoBehaviour
    {
        public DefendersMapScroller defedenderMapScrollerUp, defedenderMapScrollerRight, defedenderMapScrollerLeft, defedenderMapScrollerDown;
        public SoundMap soundMap;

        private void Start() {
            float bps = soundMap.bpm / 60 * (float)soundMap.difficulty;
            defedenderMapScrollerUp.StartMap(bps);
            defedenderMapScrollerDown.StartMap(bps);
            defedenderMapScrollerRight.StartMap(bps);
            defedenderMapScrollerLeft.StartMap(bps);
            SoundManager.Instance.PlayBackgroundMusicInstantly(soundMap.mapSong);
        }
    }
}

