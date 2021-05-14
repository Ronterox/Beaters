using ScriptableObjects;
using UnityEngine;
using Utilities;

namespace General
{
    [CreateAssetMenu(fileName = "New Song", menuName = "Map Maker/New Song")]
    public class Song : ScriptableObject
    {
        public Sprite songImage;
        public SoundMap soundMap;
        [Space]
        public ScriptableItem requiredItems;
        
        public ushort ID => soundMap.ID;
    }
}
