namespace RSLib.Framework.Collections
{
	/// <summary>
	/// Custom generic type structure, similar to list where each item has an attached probability.
	/// Sum of all probabilities do not have to be equal to 1 when added, the class will normalize all the probabilities by itself.
	/// Use the method Peek to get a random element, based on those probabilities.
	/// </summary>
	public class WeightedList<T> where T : class
    {
		public class ProbableItem
		{
            public ProbableItem(T value, float probability)
			{
                Value = value;
                Probability = probability;
			}

			public T Value { get; private set; }
			public float Probability { get; private set; }
		}

		private static System.Random s_rnd = new System.Random();

		private System.Collections.Generic.List<ProbableItem> _items = new System.Collections.Generic.List<ProbableItem>();
        private System.Collections.Generic.List<float> _probabilities = new System.Collections.Generic.List<float>();

        public WeightedList()
		{
		}

        public int Count => _items.Count;

		/// <summary>
		/// Adds an item to the list, specifying its value and its drop chance.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="probability">The drop chance, that should not be less or equal to 0.</param>
		public void Add(T value, float probability)
		{
            UnityEngine.Assertions.Assert.IsNotNull(value, "Can't add a null item.");
            UnityEngine.Assertions.Assert.IsFalse(Contains(value), "Trying to add an item that is already in the list.");

			_items.Add(new ProbableItem(value, probability));
		}

		/// <summary>
		/// Searches for the item with a specyfied value in the list and removes it.
		/// </summary>
		/// <param name="value">The probable item's value.</param>
		public void Remove(T value)
		{
            UnityEngine.Assertions.Assert.IsNotNull(value, "Can't remove an item using a null reference.");
            UnityEngine.Assertions.Assert.IsTrue(Contains(value), "Could not find a fitting item to remove.");

			_items.Remove(_items.Find(o => o.Value.Equals(value)));
		}

		/// <summary>
		/// Clears the items and probabilities lists.
		/// </summary>
		public void Clear()
		{
			_items.Clear();
			_probabilities.Clear();
		}

		/// <summary>
		/// Checks if a value is contained among the items list.
		/// </summary>
		/// <param name="value">The value to search for.</param>
		/// <returns>True if list contains the value.</returns>
		public bool Contains(T value)
		{
			return _items.Find(i => i.Value.Equals(value)) != null;
		}

		/// <summary>
		/// Peek a random item based on the probabilities distribution.
		/// </summary>
		/// <returns>The dropped item.</returns>
		public T Peek()
		{
			RefreshProbabilities();
			float p = s_rnd.Next(101) * 0.01f;

			for (int i = 0; i < _probabilities.Count; ++i)
				if (p < _probabilities[i])
					return _items[i].Value;

			return _items[Count - 1].Value;
		}

		/// <summary>
		/// Tries to get the drop chance of an item.
		/// </summary>
		/// <param name="value">The value to search for.</param>
		/// <param name="probability">The output probability if the value is in the list.</param>
		/// <returns>True if the value has been found.</returns>
		public bool TryGetProbability(T value, out float probability)
		{
			probability = 0f;

			ProbableItem item = _items.Find(i => i.Value.Equals(value));
			if (item == null)
				return false;

			probability = item.Probability;
			return true;
		}

		/// <summary>
		/// Gets all the values contained in the list.
		/// </summary>
		/// <returns>All values as a new array.</returns>
		public T[] ValuesToArray()
		{
			T[] values = new T[Count];
			for (int i = 0; i < Count; ++i)
				values[i] = _items[i].Value;

			return values;
		}

		/// <summary>
		/// Refreshes the probabilities array according to the current items.
		/// </summary>
		private void RefreshProbabilities()
		{
			_probabilities.Clear();

			float total = 0f;
            for (int i = Count - 1; i >= 0; --i)
				total += _items[i].Probability;

			for (int i = 0; i < Count; ++i)
				_probabilities.Add(_items[i].Probability + (i != 0 ? _probabilities[i - 1] : 0));
			for (int i = 0; i < Count; ++i)
				_probabilities[i] /= total;
		}
	}
}