using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Plugins.Properties;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Skill", menuName = "Skills/New skill")]
    public class ScriptableSkill : ScriptableObject
    {
        public enum Kind { Passive, Active }

        public int id;
        public Kind kind = Kind.Passive;
        public bool hasDuration;
        public string effect;

        [ConditionalHide("hasDuration")]
        public int duration;
    }

}
