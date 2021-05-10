using Plugins.Tools;
using UnityEngine;
using Utilities;

namespace Managers
{
    public enum Difficulty { Normal = 2, Hard = 3, Hardcore = 4 }

    public class GameManager : PersistentSingleton<GameManager>
    {
        public GameObject endGamePanel;
        private SoundMap m_SoundMap;

        public void ShowEndGameplayPanel(Canvas parentCanvas)
        {
            Instantiate(endGamePanel, parentCanvas.transform);
            //Give prizes
            //Get panel stars or whatever
        }

        public static void MissArrow() => DataManager.playerData.tapsDone++;

        public static void HitArrow() => DataManager.playerData.tapsDone++;

        public static void PutSoundMap(SoundMap soundMap) => Instance.m_SoundMap = soundMap;

        public static SoundMap GetSoundMap() => Instance.m_SoundMap;
    }
}
