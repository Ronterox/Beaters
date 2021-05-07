using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Rune", menuName = "Runes/New Rune")]
    public class ScriptableRune : ScriptableObject
    {

        public enum runeSet
        {
            Points, PercentualPoints, Passives, Actives
        }

        public string runeName;
        public int value;
        public float probability;
        public ushort ID { get; private set; }
        public string effect;
        public Sprite spriteOfTheRune;
        public runeSet nameOfTheSet = runeSet.Points;
    }
}
