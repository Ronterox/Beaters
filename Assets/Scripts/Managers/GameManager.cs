using Plugins.Tools;

namespace Managers
{
    public enum Difficulty
    {
        Normal, Hard, Hardcore
    }

    public class GameManager : Singleton<GameManager>
    {
        public void MissArrow() => DataManager.playerData.tapsDone++;

        public void HitArrow() => DataManager.playerData.tapsDone++;
    }
}
