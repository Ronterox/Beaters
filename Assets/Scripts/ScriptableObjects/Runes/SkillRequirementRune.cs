using Managers;
using UnityEngine;

namespace ScriptableObjects.Runes
{
    public class SkillRequirementRune : ScriptableRune
    {
        [Range(0.1f, 1f)]
        public float percentageReduction;
        
        public override void ActivateRune(GameplayManager manager) => manager.skillBarSlider.maxValue *= percentageReduction;
    }
}
