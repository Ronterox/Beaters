using General;
using Managers;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Rune", menuName = "Runes/New Rune")]
    public class ScriptableRune : IdentifiedScriptable
    {
        public string runeName, effectDescription;
        public Sprite runeSprite;

        public virtual void ActivateRune(GameplayManager manager) => Debug.Log("Rune not yet implemented");
    }
}
