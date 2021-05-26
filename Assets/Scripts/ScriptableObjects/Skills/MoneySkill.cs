using Managers;
using Plugins.Tools;
using UnityEngine;

namespace ScriptableObjects.Skills
{
    [CreateAssetMenu(fileName = "New Skill", menuName = "Skills/Money Skill")]
    public class MoneySkill : ScriptableSkill
    {
        public float duration;

        public override void UseSkill(GameplayManager manager)
        {
            manager.EveryNoteGivesMoney = true;
            
            TimerUI timer = manager.skillsTimer;

            timer.SetEvents(null, () => manager.EveryNoteGivesMoney = false);
            timer.timerTime = duration + manager.DurationIncrement;
            
            timer.StartTimer();
        }
    }
}
