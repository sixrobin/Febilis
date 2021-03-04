namespace RSLib.Framework
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>
	/// Custom generic type structure, similar to a queue but where dequeued items are then reenqueued.
	/// This structure, as it is written, should probably be used for randomly peeked elements.
	/// </summary>
	/// <typeparam name="T"> Type of items stored in the loop. </typeparam>
	public sealed class Loop<T> : IEnumerable<T>, IEnumerable, IReadOnlyCollection<T>, ICollection<T> where T : System.IComparable
	{
        #region FIELDS

        private List<T> _loop = new List<T>();
		private int _peeksCount = 0;

        #endregion FIELDS

        #region CONSTRUCTORS

        public Loop()
		{
		}

		public Loop(bool shuffleOnLoopCompleted)
		{
			ShuffleOnLoopCompleted = shuffleOnLoopCompleted;
		}

		public Loop(IEnumerable<T> content)
		{
			_loop = content.ToList();
		}

		public Loop(IEnumerable<T> content, bool shuffleOnLoopCompleted, bool shuffledOnInit)
		{
			_loop = content.ToList();
			ShuffleOnLoopCompleted = shuffleOnLoopCompleted;

			if (shuffledOnInit)
				Shuffle();
		}

		#endregion CONSTRUCTORS

		#region EVENTS

		public delegate void LoopPointReachedEventHandler();

		public event LoopPointReachedEventHandler LoopPointReached;

		#endregion EVENTS

		#region PROPERTIES

		public bool ShuffleOnLoopCompleted { get; set; }

		public int Count => _loop.Count;

		public bool IsReadOnly => true;

		#endregion PROPERTIES

		#region METHODS

		public void Add(T element)
		{
			_loop.Add(element);
		}

		public void AddRange(IEnumerable<T> collection)
		{
			foreach (T element in collection.ToArray())
				_loop.Add(element);
		}

		public void Clear()
		{
			_loop.Clear();
		}

		public bool Contains(T element)
		{
			return _loop.Contains(element);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _loop.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _loop.GetEnumerator();
		}

		/// <summary>
		/// Peeks the next loop item and sends it to the end of the structure.
		/// Also checks if a complete loop has been done, and shuffles if needed.
		/// </summary>
		/// <returns>Next item.</returns>
		public T Next()
		{
			if (_loop.Count == 0)
				return default;

			_peeksCount++;

			T item = _loop[0];
			_loop.RemoveAt(0);
			_loop.Add(item);

			if (_peeksCount == Count)
			{
				_peeksCount = 0;
				if (ShuffleOnLoopCompleted)
					Shuffle();

				LoopPointReached?.Invoke();
			}

			return item;
		}

		public void Remove(T element)
		{
			if (Contains(element))
				_loop.Remove(element);
		}

		public void Replace(T replacedElement, T newElement, bool allOccurences = true)
		{
			for (int i = 0; i < Count; ++i)
			{
				if (_loop[i].Equals(replacedElement))
				{
					_loop[i] = newElement;
					if (!allOccurences)
						return;
				}
			}
		}

		public void Shuffle()
		{
			System.Random rnd = new System.Random();
			int n = Count;
			while (n > 1)
			{
				int rndNb = rnd.Next(n--);
				(_loop[rndNb], _loop[n]) = (_loop[n], _loop[rndNb]);
			}
		}

		public void Sort()
		{
			_loop.Sort();
		}

		public void Sort(IComparer<T> comparer)
		{
			_loop.Sort(comparer);
		}

		public void Sort(System.Comparison<T> comparison)
		{
			_loop.Sort(comparison);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			_loop.CopyTo(array, arrayIndex);
		}

		bool ICollection<T>.Remove(T element)
		{
			bool result = false;

			if (Contains(element))
			{
				result = true;
				_loop.Remove(element);
			}

			return result;
		}

		public override string ToString()
		{
			string str = string.Empty;

			for (int i = 0; i < _loop.Count; ++i)
				str += _loop[i].ToString() + (i == Count - 1 ? "" : ", ");

			return str;
		}
		
		#endregion METHODS
	}
}