using Managers;

namespace ScriptableObjects.Runes
{
    public class DurationRune : ScriptableRune
    {
        public float durationIncrement;
        
        public override void ActivateRune(GameplayManager manager) => manager.DurationIncrement += durationIncrement;
    }
}
