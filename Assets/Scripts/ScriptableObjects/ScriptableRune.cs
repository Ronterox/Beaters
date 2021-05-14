using General;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Rune", menuName = "Runes/New Rune")]
    public class ScriptableRune : IdentifiedScriptable
    {
        public enum RuneSet { Points, PercentualPoints, Passives, Actives }

        public string runeName, effect;
        public int value;
        public float probability;
        public Sprite runeSprite;
        public RuneSet nameOfTheSet = RuneSet.Points;
    }
}
