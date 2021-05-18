using Plugins.Tools;
using UnityEngine;

namespace General
{
    public class IdentifiedScriptable : ScriptableObject
    {
        public ushort ID => name.GetHashCodeUshort();
    }
}
