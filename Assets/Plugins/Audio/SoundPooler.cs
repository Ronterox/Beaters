using UnityEngine;

namespace Plugins.Audio
{
    #region Pool classes

    public class SoundPoolPendingTask : BasePendingTask<SoundInstancePool, SoundPoolObject, SoundPoolConfig> { public SoundItem Data; }

    [System.Serializable]
    public class SoundPoolConfig : BasePoolConfig<SoundInstancePool, SoundPoolObject, SoundPoolConfig> { }

    public class SoundInstancePool : BasePoolInstance<SoundInstancePool, SoundPoolObject, SoundPoolConfig>
    {
        protected override void Start()
        {
            base.Start();
            m_PoolContent.parent = transform;
        }
    }

    public class SoundPoolObject : BasePoolItem<SoundInstancePool, SoundPoolObject, SoundPoolConfig>
    {
        public AudioSource audioSource;

        protected override void SetReferences()
        {
            base.SetReferences();

            audioSource = instance.GetComponent<AudioSource>();
        }

        public void SetValues(SoundPoolPendingTask task)
        {
            audioSource.transform.position = task.Position;
            audioSource.clip = task.Data.clip;
            audioSource.volume = task.Data.volume;
            audioSource.outputAudioMixerGroup = task.Data.audioMixerGroup;
            audioSource.spatialBlend = task.Data.spatialBlend;
            audioSource.loop = task.Data.loop;
            audioSource.minDistance = task.Data.minDistance;
            audioSource.maxDistance = task.Data.maxDistance;
            audioSource.rolloffMode = task.Data.audioRollOffMode;

            float range = task.Data.randomPitchRange;
            if (range != 0) audioSource.pitch = Random.Range(1 - range, 1 + range);
        }
    }

    #endregion

    /// <summary>
    /// SoundPooler class controls the pool, which sounds are currently playing, which are waiting to be played,
    /// and it also controls when an item is popped or pushed from the SoundInstancePooler.
    /// </summary>
    public class SoundPooler : Pooler<SoundPoolPendingTask, SoundInstancePool, SoundPoolObject, SoundPoolConfig>
    {
        public override void Trigger(string objectName, Vector3 position, float startDelay, params object[] additionalParams)
        {
            if (m_Pools == null) return;
            if (!m_Pools.TryGetValue(objectName.GetHashCode(), out SoundInstancePool pooler))
            {
                Debug.LogError($"SoundPooler({gameObject.name}): Pool with objectName: {objectName} wasn't found.");
                return;
            }

            if (!(additionalParams[0] is SoundItem item)) return;

            var task = new SoundPoolPendingTask
            {
                InstancePool = pooler,
                Data = item,
                Position = position,
                StartAt = Time.time + startDelay,
                EndAt = item.loop ? Mathf.Infinity : Time.time + startDelay + item.clip.length,
                PrefabName = objectName
            };

            if (startDelay > 0) m_Pending.Push(task);
            else SetUpInstance(task);
        }

        protected override void SetUpInstance(SoundPoolPendingTask task)
        {
            m_Pools.TryGetValue(task.PrefabName.GetHashCode(), out SoundInstancePool pooler);

            // Wake up object item
            if (!pooler) return;

            SoundPoolObject instance = pooler.Pop();

            instance.SetValues(task);
            task.ChainedItem = instance;
            task.ChainedItem.audioSource.Play();
            m_Running.Push(task);
        }
    }
}
