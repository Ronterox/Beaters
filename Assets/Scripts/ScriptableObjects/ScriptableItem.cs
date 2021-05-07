using Plugins.Tools;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Items/New Item")]
    public class ScriptableItem : ScriptableObject
    {
        public string itemName;
        public Character[] associatedCharacter;
        public int value;
        public float probability;
        public ushort ID => itemName.GetHashCodeUshort();
    }
}
