using System;
using Managers;
using Plugins.Tools;
using ScriptableObjects;
using UnityEngine;

namespace Skills
{
    [CreateAssetMenu(fileName = "New Skill", menuName = "Skills/Imunity Skill")]
    public class InmunitySkill : ScriptableSkill
    {
        public float duration;
        
        /// <summary>
        /// 
        /// </summary>
        public override void UseSkill()
        {
            GameplayManager gameplayManager = GameplayManager.Instance;

            gameplayManager.CanMiss = false;

            Action canMissAgain = () => gameplayManager.CanMiss = true;

            canMissAgain.DelayAction(duration);
        }
    }
}
