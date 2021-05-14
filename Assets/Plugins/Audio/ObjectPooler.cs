using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Plugins.Audio
{
    [System.Serializable]
    public class ObjectPoolItem
    {
        public GameObject ObjectToPool;
        public int AmountToPool;
        public bool ShouldExpand = true;

        public ObjectPoolItem(GameObject obj, int amt, bool exp = true)
        {
            ObjectToPool = obj;
            AmountToPool = Mathf.Max(amt, 2);
            ShouldExpand = exp;
        }
    }

    [AddComponentMenu("Petoons Studio/Tools/Object Pooler")]
    public class ObjectPooler : MonoBehaviour
    {
        public List<ObjectPoolItem> ItemsToPool;
        public string PoolName;

        private List<List<GameObject>> m_PooledObjectsList;
        private List<GameObject> m_PooledObjects;
        private List<int> m_Positions;
        private Transform m_PoolContent;

        /// <summary>
        /// Awake
        /// </summary>
        void Awake()
        {
            m_PooledObjectsList = new List<List<GameObject>>();
            m_PooledObjects = new List<GameObject>();
            m_Positions = new List<int>();

            if (PoolName != string.Empty)
            {
                m_PoolContent = new GameObject(PoolName).transform;
                Scene scene = SceneManager.GetSceneByName("GameCommon");
                if (scene.IsValid()) SceneManager.MoveGameObjectToScene(m_PoolContent.gameObject, SceneManager.GetSceneByName("GameCommon"));
            }
            for (int i = 0; i < ItemsToPool.Count; i++)
            {
                ObjectPoolItemToPooledObject(i);
            }
        }

        /// <summary>
        /// Get pooled object
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public GameObject GetPooledObject(int index)
        {
            int curSize = m_PooledObjectsList[index].Count;
            for (int i = m_Positions[index] + 1; i < m_Positions[index] + m_PooledObjectsList[index].Count; i++)
            {

                if (!m_PooledObjectsList[index][i % curSize].activeInHierarchy)
                {
                    m_Positions[index] = i % curSize;
                    return m_PooledObjectsList[index][i % curSize];
                }
            }

            if (ItemsToPool[index].ShouldExpand)
            {
                GameObject obj;
                if (PoolName != string.Empty)
                {
                    obj = (GameObject)Instantiate(ItemsToPool[index].ObjectToPool, m_PoolContent);
                } else
                {
                    obj = (GameObject)Instantiate(ItemsToPool[index].ObjectToPool, this.transform);
                } 
                obj.SetActive(false);
                m_PooledObjectsList[index].Add(obj);
                return obj;

            }
            return null;
        }

        /// <summary>
        /// Get a list of all pooled objects
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public List<GameObject> GetAllPooledObjects(int index)
        {
            return m_PooledObjectsList[index];
        }

        /// <summary>
        /// Add new object to pooling
        /// </summary>
        /// <param name="GO"></param>
        /// <param name="amt"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public int AddObject(GameObject GO, int amt = 3, bool exp = true)
        {
            ObjectPoolItem item = new ObjectPoolItem(GO, amt, exp);
            int currLen = ItemsToPool.Count;
            ItemsToPool.Add(item);
            ObjectPoolItemToPooledObject(currLen);
            return currLen;
        }

        /// <summary>
        /// Transform pooled item to pooled obj
        /// </summary>
        /// <param name="index"></param>
        private void ObjectPoolItemToPooledObject(int index)
        {
            ObjectPoolItem item = ItemsToPool[index];

            m_PooledObjects = new List<GameObject>();
            for (int i = 0; i < item.AmountToPool; i++)
            {
                GameObject obj;
                if (PoolName != string.Empty)
                {
                    obj = (GameObject)Instantiate(item.ObjectToPool, m_PoolContent);
                }
                else
                {
                    obj = (GameObject)Instantiate(item.ObjectToPool, this.transform);
                }
                obj.SetActive(false);
                m_PooledObjects.Add(obj);
            }
            m_PooledObjectsList.Add(m_PooledObjects);
            m_Positions.Add(0);
        }
    }
}

