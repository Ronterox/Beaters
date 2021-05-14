using System;
using Managers;
using Plugins.Tools;
using ScriptableObjects;
using UnityEngine;

namespace Skills
{
    [CreateAssetMenu(fileName = "New Skill", menuName = "Skills/Multiplier Skill")]
    public class MultiplierSkill : ScriptableSkill
    {
        public float multiplierValue, duration;

        public override void UseSkill()
        {
            GameplayManager gameplayManager = GameplayManager.Instance;
            
            gameplayManager.Multiplier += multiplierValue;
            
            Action deactivateMultiplier = () => gameplayManager.Multiplier -= multiplierValue;
            
            deactivateMultiplier.DelayAction(duration);
        }
    }
}
