using System;
using System.Collections.Generic;

namespace Plugins.Audio
{
    /// <summary>
    /// A fast priority queue or heap implementation.
    /// </summary>
    public class PriorityQueue<T> where T : IComparable<T>
    {
        /// <summary>
        /// The internal list used by the queue. Use with care.
        /// </summary>
        public readonly List<T> items;

        public bool Contains(T item) => items.Contains(item);

        public PriorityQueue() => items = new List<T>();

        public bool Empty => items.Count == 0;

        public T First => items.Count > 1 ? items[0] : items[items.Count - 1];

        public void Push(T item)
        {
            lock (this)
            {
                items.Add(item);
                SiftDown(0, items.Count - 1);
            }
        }

        public T Pop()
        {
            T item;

            T last = items[items.Count - 1];
            items.RemoveAt(items.Count - 1);

            if (items.Count > 0)
            {
                item = items[0];
                items[0] = last;
                SiftUp(0);
            }
            else item = last;

            return item;
        }

        private int Compare(T A, T B) => A.CompareTo(B);

        private void SiftDown(int startPos, int pos)
        {
            T newItem = items[pos];
            while (pos > startPos)
            {
                int parentPos = (pos - 1) >> 1;
                T parent = items[parentPos];

                if (Compare(parent, newItem) <= 0) break;

                items[pos] = parent;
                pos = parentPos;
            }
            items[pos] = newItem;
        }

        private void SiftUp(int pos)
        {
            int endPos = items.Count;
            int startPos = pos;
            T newItem = items[pos];
            int childPos = 2 * pos + 1;
            while (childPos < endPos)
            {
                int rightPos = childPos + 1;
                if (rightPos < endPos && Compare(items[rightPos], items[childPos]) <= 0) childPos = rightPos;

                items[pos] = items[childPos];
                pos = childPos;

                childPos = 2 * pos + 1;
            }
            items[pos] = newItem;
            SiftDown(startPos, pos);
        }

        public void Clear() => items.Clear();
    }
}
