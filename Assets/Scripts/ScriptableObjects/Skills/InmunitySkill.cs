using Managers;
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
            manager.SetSkillTimer(duration, () => manager.CanMiss = true);
        }
    }
}
