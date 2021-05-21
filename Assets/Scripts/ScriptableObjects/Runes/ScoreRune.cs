using Managers;
using UnityEngine;

namespace ScriptableObjects.Runes
{
    public class ScoreRune : ScriptableRune
    {
        [Range(0.1f, 1f)]
        public float multiplierPercentage;
        public override void ActivateRune(GameplayManager manager) => manager.Multiplier += multiplierPercentage;
    }
}
