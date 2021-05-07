using System.Collections;
using System.Collections.Generic;
using Plugins.Tools;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Rune", menuName = "Runes/New Rune")]
    public class ScriptableRune : ScriptableObject
    {
        public enum RuneSet { Points, PercentualPoints, Passives, Actives }

        public string runeName, effect;
        public int value;
        public float probability;
        public Sprite spriteOfTheRune;
        public RuneSet nameOfTheSet = RuneSet.Points;

        public ushort ID => runeName.GetHashCodeUshort();
    }
}
