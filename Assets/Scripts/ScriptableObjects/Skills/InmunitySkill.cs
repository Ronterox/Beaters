using Managers;
using Plugins.Tools;
using UnityEngine;

namespace ScriptableObjects.Skills
{
    [CreateAssetMenu(fileName = "New Skill", menuName = "Skills/Imunity Skill")]
    public class InmunitySkill : ScriptableSkill
    {
        public float duration;
        
        public override void UseSkill(GameplayManager manager)
        {
            manager.CanMiss = false;

            TimerUI timer = manager.skillsTimer;

            timer.SetEvents(null, () => manager.CanMiss = true);
            timer.timerTime = duration + manager.DurationIncrement;
            
            timer.StartTimer();
        }
    }
}
