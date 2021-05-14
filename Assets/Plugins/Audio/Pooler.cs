using System.Collections.Generic;
using UnityEngine;

namespace Plugins.Audio
{
    /// <summary>
    /// Base Pending Task for delay pools
    /// </summary>
    public class BasePendingTask<TInstancePooler, TPoolItem, TConfig> : System.IComparable<BasePendingTask<TInstancePooler, TPoolItem, TConfig>>
        where TInstancePooler : ObjectPool<TInstancePooler, TPoolItem, TConfig>
        where TConfig : PoolConfig<TInstancePooler, TPoolItem, TConfig>
        where TPoolItem : PoolObject<TInstancePooler, TPoolItem, TConfig>, new()
    {
        public Vector3 Position;
        public float StartAt;
        public float EndAt;
        public string PrefabName;

        public TPoolItem ChainedItem;

        public ObjectPool<TInstancePooler, TPoolItem, TConfig> InstancePool;

        public int CompareTo(BasePendingTask<TInstancePooler, TPoolItem, TConfig> other) => StartAt.CompareTo(other.StartAt);
    }

    public class BasePoolConfig<TInstancePooler, TPoolItem, TConfig> : PoolConfig<TInstancePooler, TPoolItem, TConfig>
        where TInstancePooler : ObjectPool<TInstancePooler, TPoolItem, TConfig>
        where TConfig : PoolConfig<TInstancePooler, TPoolItem, TConfig>
        where TPoolItem : PoolObject<TInstancePooler, TPoolItem, TConfig>, new()
    {

    }

    public class BasePoolInstance<TInstancePooler, TPoolItem, TConfig> : ObjectPool<TInstancePooler, TPoolItem, TConfig>
        where TInstancePooler : ObjectPool<TInstancePooler, TPoolItem, TConfig>
        where TConfig : PoolConfig<TInstancePooler, TPoolItem, TConfig>
        where TPoolItem : PoolObject<TInstancePooler, TPoolItem, TConfig>, new()
    {

    }

    public class BasePoolItem<TInstancePooler, TPoolItem, TConfig> : PoolObject<TInstancePooler, TPoolItem, TConfig>
        where TInstancePooler : ObjectPool<TInstancePooler, TPoolItem, TConfig>
        where TConfig : PoolConfig<TInstancePooler, TPoolItem, TConfig>
        where TPoolItem : PoolObject<TInstancePooler, TPoolItem, TConfig>, new()
    {

    }

    /// <summary>
    /// Abstract pooler generator
    /// </summary>
    /// <typeparam name="TPooledType"></typeparam>
    /// <typeparam name="TInstancePooler"></typeparam>
    /// <typeparam name="TPoolItem"></typeparam>
    /// <typeparam name="TConfig"></typeparam>
    public abstract class Pooler<TPooledType, TInstancePooler, TPoolItem, TConfig> : MonoBehaviour 
        where TPooledType : BasePendingTask<TInstancePooler, TPoolItem, TConfig>
        where TInstancePooler : BasePoolInstance<TInstancePooler, TPoolItem, TConfig>
        where TConfig : BasePoolConfig<TInstancePooler, TPoolItem, TConfig>
        where TPoolItem : BasePoolItem<TInstancePooler, TPoolItem, TConfig>, new()
    {
        protected PriorityQueue<TPooledType> m_Running, m_Pending;
        protected Dictionary<int, TInstancePooler> m_Pools;

        public TConfig[] PrefabConfig;

        /// <summary>
        /// Start
        /// </summary>
        protected virtual void Awake() => Setup();

        /// <summary>
        /// Setup
        /// </summary>
        protected virtual void Setup()
        {
            m_Pools = new Dictionary<int, TInstancePooler>();
            foreach (TConfig item in PrefabConfig)
            {
                item.pool = gameObject.AddComponent<TInstancePooler>();
                item.pool.prefab = item.Prefab;
                item.pool.initialPoolCount = item.InitialObjectCount;
                item.pool.PoolName = item.PoolName;

                m_Pools[item.Prefab.name.GetHashCode()] = item.pool as TInstancePooler;
            }

            m_Running = new PriorityQueue<TPooledType>();
            m_Pending = new PriorityQueue<TPooledType>();
        }

        /// <summary>
        /// Update
        /// </summary>
        private void Update()
        {
            
            // Sleep pooled item if expire time passed
            while (!m_Running.Empty && m_Running.First.EndAt <= Time.time)
            {
                TPooledType instance = m_Running.Pop();
                instance.ChainedItem.ReturnToPool();
            }

            // Pool item destroyed/inactive before expiration must become sleep
            while (!m_Running.Empty && !m_Running.First.ChainedItem.instance.gameObject.activeSelf)
            {
                TPooledType instance = m_Running.Pop();
                instance.ChainedItem.ReturnToPool();
            }

            // Check pending list and move to running if required
            while (!m_Pending.Empty && m_Pending.First.StartAt <= Time.time)
            {
                TPooledType task = m_Pending.Pop();
                SetUpInstance(task);
            }
            
        }

        /// <summary>
        /// Wake up pool item & initialize values
        /// </summary>
        /// <param name="task"></param>
        protected abstract void SetUpInstance(TPooledType task);

        /// <summary>
        /// public call for triggering pool
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="position"></param>
        /// <param name="startDelay"></param>
        /// <param name="additionalParams"></param>
        public abstract void Trigger(string objectName, Vector3 position, float startDelay, params object[] additionalParams);

        /// <summary>
        /// Stop pooler
        /// </summary>
        public void Stop()
        {
            // Sleep pooled item if expire time passed
            while (!m_Running.Empty && m_Running.First.EndAt <= Time.time)
            {
                TPooledType instance = m_Running.Pop();
                instance.InstancePool.Push(instance.ChainedItem);
            }

            m_Running.Clear();
            m_Pending.Clear();
        }
    }
}