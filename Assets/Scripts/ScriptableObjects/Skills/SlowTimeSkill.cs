using Managers;
using UnityEngine;

namespace ScriptableObjects.Skills
{
    [CreateAssetMenu(fileName = "New Slow Time Skill", menuName = "Skills/Slow Time Skill")]
    public class SlowTimeSkill : ScriptableSkill
    {
        public float duration;
        
        public override void UseSkill(GameplayManager manager) => manager.SlowTime(duration + manager.DurationIncrement, 0f);
    }
}
