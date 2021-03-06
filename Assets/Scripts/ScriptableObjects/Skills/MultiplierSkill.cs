using Managers;
using UnityEngine;

namespace ScriptableObjects.Skills
{
    [CreateAssetMenu(fileName = "New Skill", menuName = "Skills/Multiplier Skill")]
    public class MultiplierSkill : ScriptableSkill
    {
        public float multiplierValue, duration;

        public override void UseSkill(GameplayManager manager)
        {
            manager.Multiplier += multiplierValue;

            manager.PlayAnimationPrefab(skillAnimationPrefab, sfx, () => manager.SetSkillTimer(duration, () => manager.Multiplier -= multiplierValue));
        }
    }
}
