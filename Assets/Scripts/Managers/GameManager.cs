using General;
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
        private Song m_Song;

        public void ShowEndGameplayPanel(Canvas parentCanvas)
        {
            Instantiate(endGamePanel, parentCanvas.transform);
            //Give prizes
            //Get panel stars or whatever
        }

        public static void MissArrow() => DataManager.playerData.tapsDone++;

        public static void HitArrow()
        {
            DataManager.playerData.tapsDone++;
            
            Song song = m_Instance.m_Song;
            
            if (song)
            {
                //Check for probability of gain money/prize    
            }
        }

        public static void PutSoundMap(SoundMap soundMap) => m_Instance.m_SoundMap = soundMap;

        public static void PutSoundMap(Song song) => m_Instance.m_Song = song;

        public static SoundMap GetSoundMap() => m_Instance.m_SoundMap ?? m_Instance.m_Song.soundMap;
    }
}
