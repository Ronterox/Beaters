using General;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Items/New Item")]
    public class ScriptableItem : IdentifiedScriptable
    {
        public string itemName;
        public Sprite itemSprite;
    }
}
