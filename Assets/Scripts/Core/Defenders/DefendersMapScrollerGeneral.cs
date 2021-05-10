using Managers;
using UnityEngine;
using Plugins.Audio;
using Utilities;

namespace Core.Defenders
{
    public class DefendersMapScrollerGeneral : MonoBehaviour
    {
        public DefendersMapScroller scrollerUp, scrollerRight, scrollerLeft, scrollerDown;
        public Difficulty difficulty;
        public SoundMap soundMap;

        private void Start()
        {
            float bps = soundMap.bpm / 60 * (float)difficulty;
            scrollerUp.StartMap(bps);
            scrollerDown.StartMap(bps);
            scrollerRight.StartMap(bps);
            scrollerLeft.StartMap(bps);

            SoundManager.Instance.PlayBackgroundMusicNoFade(soundMap.audioClip);
        }
    }
}
