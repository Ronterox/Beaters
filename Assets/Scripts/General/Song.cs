using Plugins.Tools;
using UnityEngine;
using Utilities;

namespace General
{
    public class Song : ScriptableObject
    {
        public string songName;
        public SoundMap soundMap;

        public ushort ID => songName.GetHashCodeUshort();
    }
}
