namespace RSLib.Jumble
{
	public class CheckChangeArray<T> where T : class
	{
        public T[] _array;
        private T[] _arrayCopy;

        public int Length => _array.Length;

        public T this[int index]
        {
            get => _array[index];
            set => _array[index] = value;
        }

        public CheckChangeArray()
		{
		}

		public CheckChangeArray(int length)
		{
			_array = new T[length];
			_arrayCopy = new T[length];
		}

		public CheckChangeArray(T[] array)
		{
			_array = new T[array.Length];
			_arrayCopy = new T[array.Length];
			System.Array.Copy(array, _array, Length);
			System.Array.Copy(_array, _arrayCopy, Length);
		}

		/// <summary>
		/// Checks both the actual array and the copy.
		/// </summary>
		/// <returns>Tuple with new and old value if change is detected, else null.</returns>
		public System.Tuple<T, T> CheckChange()
		{
			for (int i = 0; i < Length; ++i)
            {
				if (!_array[i].Equals(_arrayCopy[i]))
				{
                    System.Tuple<T, T> change = new System.Tuple<T, T>(_array[i], _arrayCopy[i]);
					System.Array.Copy(_array, _arrayCopy, _array.Length);
					return change;
				}
            }

			return null;
		}
	}
}