using Plugins.Tools;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Items/New Item")]
    public class ScriptableItem : ScriptableObject
    {
        public string itemName;
        public Sprite itemSprite;
        public ushort ID => itemName.GetHashCodeUshort();
    }
}
