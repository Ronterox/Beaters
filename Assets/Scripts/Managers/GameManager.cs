using Plugins.Tools;

namespace Managers
{
    public enum Difficulty { Normal = 2, Hard = 3, Hardcore = 4 }

    public class GameManager : Singleton<GameManager>
    {
        public void MissArrow() => DataManager.playerData.tapsDone++;

        public void HitArrow() => DataManager.playerData.tapsDone++;
    }
}
