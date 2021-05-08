using UnityEngine;
using Utilities;

namespace General
{
    [CreateAssetMenu(fileName = "New Song", menuName = "Map Maker/New Song")]
    public class Song : ScriptableObject
    {
        public SoundMap soundMap;
        public ushort ID => soundMap.ID;
    }
}
