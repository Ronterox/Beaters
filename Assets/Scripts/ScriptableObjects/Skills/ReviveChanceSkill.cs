using General;
using Managers;
using Plugins.Audio;
using UnityEngine;

namespace ScriptableObjects.Skills
{
    [CreateAssetMenu(fileName = "New Revive Chance Skill", menuName = "Skills/Revive Chance Skill")]
    public class ReviveChanceSkill : ScriptableSkill
    {
        [Range(0f, 1f)]
        public float reviveProbability;

        public override void UseSkill(GameplayManager manager)
        {
            Character character = manager.currentCharacter;
            character.onDie -= manager.Lose;
            character.onDie += () =>
            {
                if (Random.Range(0f, 1f) <= reviveProbability)
                {
                    character.Revive(10);
                    manager.PlayAnimationPrefab(skillAnimationPrefab, sfx);
                }
                else manager.Lose();
            };
        }
    }
}
