using Managers;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Skill", menuName = "Skills/New skill")]
    public class ScriptableSkill : ScriptableObject
    {
        [TextArea]
        public string effectDescription;
        public int rechargeQuantity;
        [Space]
        public Sprite skillImage;
        public AudioClip sfx;
        [Space]
        public GameObject skillAnimationPrefab;

        public virtual void UseSkill(GameplayManager manager) => Debug.Log($"Not implemented the skill {name}");
    }
}
