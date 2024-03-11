namespace RSLib.Framework.Collections
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// ConcurrentQueue wrapper that automatically dequeues item if size exceeds a limit.
    /// </summary>
    public sealed class FixedSizedConcurrentQueue<T> : IReadOnlyCollection<T>
    {
        private System.Collections.Concurrent.ConcurrentQueue<T> _concurrentQueue = new System.Collections.Concurrent.ConcurrentQueue<T>();
        private object _lock = new object();

        public FixedSizedConcurrentQueue(int fixedSize)
        {
            FixedSize = fixedSize;
        }

        public int FixedSize { get; }

        public int Count => _concurrentQueue.Count;

        public void Enqueue(T element)
        {
            _concurrentQueue.Enqueue(element);
            while (_concurrentQueue.Count > FixedSize && _concurrentQueue.TryDequeue(out _))
            {
            }
        }

        public bool TryDequeue(out T element)
        {
            return _concurrentQueue.TryDequeue(out element);
        }

        public bool TryPeek(out T element)
        {
            return _concurrentQueue.TryPeek(out element);
        }

        public T[] ToArray()
        {
            return _concurrentQueue.ToArray();
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock (_lock)
                return new List<T>(_concurrentQueue).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}