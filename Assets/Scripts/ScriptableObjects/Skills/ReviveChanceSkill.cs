using General;
using Managers;
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
                    character.currentHp = 0;
                    character.Heal(1);
                }
                else manager.Lose();
            };
        }
    }
}