using General;
using Plugins.Tools;
using Utilities;

namespace Managers
{
    public enum Difficulty { Normal = 2, Hard = 3, Hardcore = 4 }

    public class GameManager : PersistentSingleton<GameManager>
    {
        private SoundMap m_SoundMap;
        public Song Song { get; private set; }

        public static void PutSoundMap(SoundMap soundMap) => m_Instance.m_SoundMap = soundMap;

        public static void PutSoundMap(Song song) => m_Instance.Song = song;

        public static SoundMap GetSoundMap() => m_Instance.m_SoundMap ?? m_Instance.Song.soundMap;
    }
}
