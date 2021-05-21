using Plugins.Audio;
using Plugins.Tools;
using ScriptableObjects;
using UnityEngine;

namespace General
{
    public class StartSoundManager : MonoBehaviour
    {
        public Song[] songs;
        private void Start()
        {
            SoundManager soundManager = SoundManager.Instance;
            if (!soundManager.IsPlaying)
            {
                soundManager.PlayBackgroundMusicNoFade(songs.GetRandom().soundMap.audioClip);
            }
        }
    }
}
