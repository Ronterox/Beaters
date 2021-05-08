using UnityEngine;
using Utilities;

namespace General
{
    public class Song : ScriptableObject
    {
        public SoundMap soundMap;
        public ushort ID => soundMap.ID;
    }
}
