using System;
using Managers;
using Plugins.Tools;
using UnityEngine;

namespace ScriptableObjects.Skills
{
    [CreateAssetMenu(fileName = "New Skill", menuName = "Skills/Heal Skill")]
    public class HealSkill : ScriptableSkill
    {
        [Range(0.1f, 0.3f)]
        public float healingValue;

        public override void UseSkill(GameplayManager manager)
        {
            manager.comboTimeHeal = true;
            manager.healingValue = healingValue;
        }
    }
}
