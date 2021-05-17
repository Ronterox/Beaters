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

        public virtual void UseSkill() => Debug.Log($"Not implemented the skill {name}");
    }
}
