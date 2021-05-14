using ScriptableObjects;
using UnityEngine;

namespace General
{
    public class Character : MonoBehaviour
    {
        public ScriptableCharacter character;

        public int level;
        public int exp;

        public int calculateHP(int lvl) => Mathf.RoundToInt(character.hp + lvl * character.multiplier);
    }
}
