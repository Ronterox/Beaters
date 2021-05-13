using UnityEngine;
using Plugins.Properties;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Skill", menuName = "Skills/New skill")]
    public class ScriptableSkill : ScriptableObject
    {
        public enum Kind { Passive, Active }

        public Kind kind = Kind.Passive;
        [TextArea]
        public string effectDescription;
        public bool hasDuration;

        public int rechargeQuantity;

        [ConditionalHide("hasDuration")]
        public int duration;

        public virtual void UseSkill()
        {
            switch (kind)
            {
                case Kind.Active:
                    //TODO: functional skill for kpoper and metalhead
                    break;
                case Kind.Passive:
                    
                    break;
            }
        }
    }
}
