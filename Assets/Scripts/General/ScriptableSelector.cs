using System.Collections.Generic;
using Plugins.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace General
{
    public abstract class ScriptableSelector<T> : MonoBehaviour where T : IdentifiedScriptable
    {
        public Image objectImage;

        public T[] objectList;
        protected readonly List<T> m_PlayerObjects = new List<T>();
        protected int m_Index;

        private void Start()
        {
            List<ushort> objIds = GetObjectIds();

            objectList.ForEach(obj =>
            {
                if (objIds.Contains(obj.ID)) m_PlayerObjects.Add(obj);
            });

            CheckAndSetObject();
        }

        public void TravelObjects(int index)
        {
            m_Index.ChangeValueLoop(index, m_PlayerObjects.Count);
            CheckAndSetObject();
        }

        private void CheckAndSetObject()
        {
            if(m_PlayerObjects.Count < 1) return;
            SetObject();
        }

        protected abstract List<ushort> GetObjectIds();

        protected abstract void SetObject();
    }
}
