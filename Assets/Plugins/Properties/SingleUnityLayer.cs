using UnityEngine;

namespace Plugins.Properties
{
    [System.Serializable]
    public class SingleUnityLayer
    {
        [SerializeField]
        private int layerIndex;
        public int LayerIndex => layerIndex;

        public void Set(int layer)
        {
            if (layer > 0 && layer < 32)
            {
                layerIndex = layer;
            }
        }

        public int Mask => 1 << layerIndex;
    }
}
