using Managers;
using UnityEngine;

namespace ScriptableObjects.Skills
{
    [CreateAssetMenu(fileName = "New Skill", menuName = "Skills/Money Skill")]
    public class MoneySkill : ScriptableSkill
    {
        public float duration;

        public override void UseSkill(GameplayManager manager)
        {
            manager.EveryNoteGivesMoney = true;
            manager.SetSkillTimer(duration,() => manager.EveryNoteGivesMoney = false);
        }
    }
}
