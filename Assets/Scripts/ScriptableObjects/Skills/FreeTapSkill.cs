using Managers;
using UnityEngine;

namespace ScriptableObjects.Skills
{
    [CreateAssetMenu(fileName = "New Free taps skill", menuName = "Skills/Free Taps Skill")]
    public class FreeTapSkill : ScriptableSkill
    {
        public int freeTaps;
        public override void UseSkill(GameplayManager manager)
        {
            manager.SetFreeTaps(freeTaps);
            manager.PlayAnimationPrefab(skillAnimationPrefab, sfx);
        }
    }
}
