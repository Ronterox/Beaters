using Plugins.Tools;
using Utilities;

namespace Managers
{
    public enum Difficulty { Normal = 2, Hard = 3, Hardcore = 4 }

    public class GameManager : PersistentSingleton<GameManager>
    {
        private SoundMap m_SoundMap;

        public static void MissArrow() => DataManager.playerData.tapsDone++;

        public static void HitArrow() => DataManager.playerData.tapsDone++;

        public static void PutSoundMap(SoundMap soundMap) => Instance.m_SoundMap = soundMap;

        public static SoundMap GetSoundMap() => Instance.m_SoundMap;
    }
}
