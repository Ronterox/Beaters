using UnityEngine;
using Utilities;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Song", menuName = "Map Maker/New Song")]
    public class Song : ScriptableObject
    {
        public Sprite songImage;
        public SoundMap soundMap;
        [Space]
        public ScriptableItem requiredItem1, requiredItem2;
        public int requiredQuantityItem1, requiredQuantityItem2;
        
        public ushort ID => soundMap.ID;
    }
}
