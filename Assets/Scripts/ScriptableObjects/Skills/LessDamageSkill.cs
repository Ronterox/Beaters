using System;
using Managers;
using Plugins.Tools;
using UnityEngine;

namespace ScriptableObjects.Skills
{
    [CreateAssetMenu(fileName = "New Skill", menuName = "Skills/Less Damage Skill")]
    public class LessDamageSkill : ScriptableSkill
    {
        [Range(0.1f, 1f)]
        public float reduceDamage;

        public override void UseSkill(GameplayManager manager)
        {
            float percentage = manager.minimumDamage * reduceDamage;
            manager.minimumDamage -= (int)percentage;
        }
    }
}
