namespace RSLib.Framework.Collections
{
    public interface IHeapElement<T> : System.IComparable<T>
    {
        int HeapIndex { get; set; }
    }

    public class Heap<T> where T : IHeapElement<T>
    {
        private T[] _elements;

        public Heap(int maxSize)
        {
            _elements = new T[maxSize];
        }

        public int Count { get; private set; }

        /// <summary>
        /// Removes the first element in the heap tree.
        /// </summary>
        /// <returns>First element.</returns>
        public T RemoveFirst()
        {
            T first = _elements[0];
            Count--;
            _elements[0] = _elements[Count];
            _elements[0].HeapIndex = 0;
            SortDown(_elements[0]);

            return first;
        }

        /// <summary>
        /// Adds an item to the heap tree and sorts it in.
        /// </summary>
        /// <param name="element">Item to add.</param>
        public void Add(T element)
        {
            element.HeapIndex = Count;
            _elements[Count] = element;
            SortUp(element);
            Count++;
        }

        /// <summary>
        /// Checks if the heap tree contains the given element.
        /// </summary>
        /// <param name="element">Element to look for.</param>
        /// <returns>True if the heap contains the element, else false.</returns>
        public bool Contains(T element)
        {
            return Equals(_elements[element.HeapIndex], element);
        }

        /// <summary>
        /// Swaps two items positions in the heap and their indexes.
        /// </summary>
        /// <param name="a">First item.</param>
        /// <param name="b">Second item.</param>
        private void Swap(T a, T b)
        {
            (_elements[a.HeapIndex], _elements[b.HeapIndex]) = (_elements[b.HeapIndex], _elements[a.HeapIndex]);
            (a.HeapIndex, b.HeapIndex) = (b.HeapIndex, a.HeapIndex);
        }

        /// <summary>
        /// Retrieves the correct position of an item by sorting it up the heap tree.
        /// </summary>
        /// <param name="element">Item to sort up.</param>
        private void SortUp(T element)
        {
            int parentIndex = (element.HeapIndex - 1) / 2;
            while (true)
            {
                T parentItem = _elements[parentIndex];
                if (element.CompareTo(parentItem) > 0)
                    Swap(element, parentItem);
                else
                    break;

                parentIndex = (element.HeapIndex - 1) / 2;
            }
        }

        /// <summary>
        /// Retrieves the correct position of an item by sorting it down the heap tree.
        /// </summary>
        /// <param name="element">Item to sort down.</param>
        private void SortDown(T element)
        {
            while (true)
            {
                int leftChildIndex = element.HeapIndex * 2 + 1;
                int rightChildIndex = element.HeapIndex * 2 + 2;

                if (leftChildIndex >= Count)
                    return;

                int swapIndex = leftChildIndex;

                if (rightChildIndex < Count && _elements[leftChildIndex].CompareTo(_elements[rightChildIndex]) < 0)
                    swapIndex = rightChildIndex;

                if (element.CompareTo(_elements[swapIndex]) >= 0)
                    return;

                Swap(element, _elements[swapIndex]);
            }
        }
    }
}