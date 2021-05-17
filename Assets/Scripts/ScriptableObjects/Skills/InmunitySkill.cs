using System;
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

            Action canMissAgain = () => manager.CanMiss = true;

            canMissAgain.DelayAction(duration + manager.DurationIncrement);
        }
    }
}
