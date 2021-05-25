using Managers;
using UnityEngine;

namespace ScriptableObjects.Skills
{
    [CreateAssetMenu(fileName = "New Minimum Combo Skill", menuName = "Skills/Minimum Combo Skill")]
    public class MinimumComboSkill : ScriptableSkill
    {
        public int minimumCombo;
        public override void UseSkill(GameplayManager manager) => manager.MinimumCombo += minimumCombo;
    }
}
