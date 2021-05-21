using Managers;
using UnityEngine;

namespace ScriptableObjects.Skills
{
    [CreateAssetMenu(fileName = "New Skill", menuName = "Skills/Extra Score Skill")]
    public class ExtraScoreSkill : ScriptableSkill
    {
        [Range(0f, 1f)]
        public float scoreMultiplierValue;

        public override void UseSkill(GameplayManager manager)
        {
            float percentage = manager.Multiplier * scoreMultiplierValue;
            manager.Multiplier += percentage;
        }
    }
}
