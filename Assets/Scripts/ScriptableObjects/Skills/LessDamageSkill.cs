using Managers;
using UnityEngine;

namespace ScriptableObjects.Skills
{
    [CreateAssetMenu(fileName = "New Skill", menuName = "Skills/Less Damage Skill")]
    public class LessDamageSkill : ScriptableSkill
    {
        [Range(0f, 1f)]
        public float reduceDamage;

        public override void UseSkill(GameplayManager manager)
        {
            float percentage = manager.MinimumDamage * reduceDamage;
            manager.MinimumDamage -= (int)percentage;
        }
    }
}
