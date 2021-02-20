namespace RSLib.Extensions
{
	using System.Collections.Generic;

	public static class IListExtensions
	{
		private static System.Random s_rnd = new System.Random();

		#region CONVERSION

		/// <summary>Converts list to a queue.</summary>
		/// <param name="enqueueFromStart">Starts enqueuing from first element.</param>
		/// <returns>List as a new queue.</returns>
		public static Queue<T> ToQueue<T>(this IList<T> list, bool enqueueFromStart = true)
		{
			Queue<T> queue = new Queue<T>();
			for (int i = enqueueFromStart ? 0 : list.Count - 1; enqueueFromStart ? (i < list.Count) : (i >= 0); i += enqueueFromStart ? 1 : -1)
				queue.Enqueue(list[i]);

			return queue;
		}

		/// <summary>Converts list to a stack.</summary>
		/// <param name="enqueueFromStart">Start stacking from first element.</param>
		/// <returns>List as a new stack.</returns>
		public static Stack<T> ToStack<T>(this IList<T> list, bool stackFromStart = true)
		{
			Stack<T> stack = new Stack<T>();
			for (int i = stackFromStart ? 0 : list.Count - 1; stackFromStart ? (i < list.Count) : (i >= 0); i += stackFromStart ? 1 : -1)
				stack.Push(list[i]);

			return stack;
		}

		/// <summary>Writes the list elements in a string.</summary>
		/// <param name="list">List to write.</param>
		/// <returns>String with the list elements.</returns>
		public static string ToStringImproved<T>(this IList<T> list)
		{
			string str = string.Empty;
			for (int i = 0; i < list.Count; ++i)
				str += list[i].ToString() + (i == list.Count - 1 ? "" : ", ");

			return str;
		}

		/// <summary>Writes the list elements in a string using a given splitting char.</summary>
		/// <param name="list">List to write.</param>
		/// <param name="split">Splitting char.</param>
		/// <returns>String with the list elements.</returns>
		public static string ToStringImproved<T>(this IList<T> list, char split)
		{
			string str = string.Empty;
			for (int i = 0; i < list.Count; ++i)
				str += list[i].ToString() + (i == list.Count - 1 ? "" : split.ToString());

			return str;
		}

		/// <summary>Writes the list elements in a string using a given splitting string.</summary>
		/// <param name="list">List to write.</param>
		/// <param name="split">Splitting string.</param>
		/// <returns>String with the list elements.</returns>
		public static string ToStringImproved<T>(this IList<T> list, string split)
		{
			string str = string.Empty;
			for (int i = 0; i < list.Count; ++i)
				str += list[i].ToString() + (i == list.Count - 1 ? "" : string.IsNullOrEmpty(split) ? " / " : split);

			return str;
		}

		#endregion CONVERSION

		#region GENERAL

		/// <summary>Returns any randomly picked element.</summary>
		/// <param name="list">List to get any element from.</param>
		/// <returns>Any element.</returns>
		public static T Any<T>(this IList<T> list)
		{
			return list[s_rnd.Next(list.Count)];
		}

		/// <summary>
		/// Returns many randomly picked elements of a list.
		/// Returns the same list if quantity is greater than original list count.
		/// Returns an empty list if quantity is minus or equal to 0.
		/// </summary>
		/// <param name="list">List to get elements from.</param>
		/// <param name="quantity">Amount of elements to pick.</param>
		/// <returns>New array with picked elements.</returns>
		public static IList<T> Any<T>(this IList<T> list, int quantity)
		{
			if (quantity <= 0)
				return new List<T>();

			if (quantity >= list.Count)
				return list;

			IList<T> copy = list;
			IList<T> choice = new List<T>();
			for (int i = quantity - 1; i >= 0; --i)
			{
				T pick = copy.Any();
				choice.Add(pick);
				copy.Remove(pick);
			}

			return choice;
		}

		/// <summary>Shuffles the list.</summary>
		/// <param name="list">List to shuffle.</param>
		/// <returns>Shuffled list.</returns>
		public static void Shuffle<T>(this IList<T> list)
		{
			int n = list.Count;
			while (n > 1)
				list.Swap(s_rnd.Next(n--), n);
		}

		/// <summary>Shuffles the list in a new list.</summary>
		/// <param name="list">List to shuffle.</param>
		/// <returns>Shuffled list.</returns>
		public static IList<T> ShuffleIntoNewList<T>(this IList<T> list)
		{
			IList<T> copy = new List<T>();
			for (int i = list.Count - 1; i >= 0; --i)
				copy.Add(list[i]);

			int n = copy.Count;
			while (n > 1)
				copy.Swap(s_rnd.Next(n--), n);

			return copy;
		}

		/// <summary>Swaps the positions of 2 elements.</summary>
		/// <param name="first">Index of first.</param>
		/// <param name="second">Index of second.</param>
		public static void Swap<T>(this IList<T> list, int first, int second)
		{
			(list[first], list[second]) = (list[second], list[first]);
		}

		#endregion GENERAL
	}
}