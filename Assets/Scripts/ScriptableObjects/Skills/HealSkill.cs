using Managers;
using UnityEngine;

namespace ScriptableObjects.Skills
{
    [CreateAssetMenu(fileName = "New Skill", menuName = "Skills/Heal Skill")]
    public class HealSkill : ScriptableSkill
    {
        [Range(0f, 0.3f)]
        public float healingValue;

        public override void UseSkill(GameplayManager manager)
        {
            manager.ComboTimeHeal = true;
            manager.HealingValueComboTime = healingValue;
        }
    }
}
