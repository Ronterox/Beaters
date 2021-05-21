using System;
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
            manager.everyNoteGivesMoney = true;
            
            Action deactivateMultiplier = () => manager.everyNoteGivesMoney = false;
            
            deactivateMultiplier.DelayAction(duration + manager.DurationIncrement);
        }
    }
}
