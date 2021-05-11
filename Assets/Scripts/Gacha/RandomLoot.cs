using Plugins.Tools;
using UnityEngine;

namespace Gacha
{
    [System.Serializable]
    public struct LootOption
    {
        public GameObject obj;
        public float probability;
    }

    public class RandomLoot : MonoBehaviour
    {
        public LootOption[] table;

        private float m_Total, m_RandomNumber;

        public void RandomItem()
        {
            table.Shuffle();

            foreach (LootOption item in table)
            {
                m_Total += item.probability;
            }

            var rng = new System.Random(Random.Range(0, (int)m_Total));
            m_RandomNumber = rng.Next((int)m_Total);

            for (var i = 0; i < table.Length; i++)
            {
                if (m_RandomNumber <= table[i].probability)
                {
                    table[i].obj.SetActive(true);
                    return;
                }
                m_RandomNumber -= table[i].probability;
            }
        }
    }
}
