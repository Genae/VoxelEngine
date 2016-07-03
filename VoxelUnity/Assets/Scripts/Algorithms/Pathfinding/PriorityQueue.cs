using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Algorithms.Pathfinding
{
    public class PriorityQueue<T>
    {
        private int _totalSize;
        private readonly SortedDictionary<int, Queue<T>> _storage;

        public PriorityQueue()
        {
            _storage = new SortedDictionary<int, Queue<T>>();
            _totalSize = 0;
        }

        public bool IsEmpty()
        {
            return (_totalSize == 0);
        }

        public bool Contains(T obj)
        {
            return _storage.Any(q => q.Value.Contains(obj));
        }

        public T Dequeue()
        {
            if (IsEmpty())
            {
                return default(T);
            }
            foreach (var q in _storage.Values.Where(q => q.Count > 0))
            {
                _totalSize--;
                return q.Dequeue();
            }
            return default(T);
        }

        // same as above, except for peek.

        public T Peek()
        {
            if (IsEmpty())
                return default(T);
            foreach (var q in _storage.Values.Where(q => q.Count > 0))
            {
                return q.Peek();
            }
            return default(T); // not supposed to reach here.
        }

        public object Dequeue(int prio)
        {
            _totalSize--;
            return _storage[prio].Dequeue();
        }

        public void Enqueue(T item, int prio)
        {
            if (!_storage.ContainsKey(prio))
            {
                _storage.Add(prio, new Queue<T>());
            }
            _storage[prio].Enqueue(item);
            _totalSize++;

        }
    }
}