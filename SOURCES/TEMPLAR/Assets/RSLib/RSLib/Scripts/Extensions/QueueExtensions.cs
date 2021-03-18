namespace RSLib.Extensions
{
	using System.Collections.Generic;

	public static class QueueExtensions
    {
		#region CONVERSION

		/// <summary>Writes the queue elements in a string.</summary>
		/// <param name="queue">Queue to write.</param>
		/// <returns>String with the queue elements.</returns>
		public static string ToStringImproved<T>(this Queue<T> queue)
		{
			string str = string.Empty;
			int elementIndex = 0;

			foreach (T element in queue)
			{
				++elementIndex;
				str += element.ToString() + (elementIndex == queue.Count ? "" : ", ");
			}

			return str;
		}

		/// <summary>Writes the queue elements in a string using a given splitting char.</summary>
		/// <param name="queue">Queue to write.</param>
		/// <param name="split">Splitting char.</param>
		/// <returns>String with the queue elements.</returns>
		public static string ToStringImproved<T>(this Queue<T> queue, char split)
		{
			string str = string.Empty;
			int elementIndex = 0;

			foreach (T element in queue)
			{
				++elementIndex;
				str += element.ToString() + (elementIndex == queue.Count ? "" : split.ToString());
			}

			return str;
		}

		/// <summary>Writes the queue elements in a string using a given splitting string.</summary>
		/// <param name="queue">Queue to write.</param>
		/// <param name="split">Splitting string.</param>
		/// <returns>String with the queue elements.</returns>
		public static string ToStringImproved<T>(this Queue<T> queue, string split)
		{
			string str = string.Empty;
			int elementIndex = 0;

			foreach (T element in queue)
			{
				++elementIndex;
				str += element.ToString() + (elementIndex == queue.Count ? "" : string.IsNullOrEmpty(split) ? " / " : split);
			}

			return str;
		}

		#endregion CONVERSION

		#region GENERAL

		/// <summary>Enqueues all the elements of an IEnumerable.</summary>
		/// <param name="collection">The IEnumerable to enqueue elements from.</param>
		public static void Enqueue<T>(this Queue<T> queue, IEnumerable<T> collection)
		{
			foreach (T element in collection)
				queue.Enqueue(element);
		}

		/// <summary>Loops through all elements in the queue and executes an action.</summary>
		/// <param name="action">Action to execute.</param>
		public static void ForEach<T>(this Queue<T> queue, System.Action<T> action)
		{
			foreach (T element in queue)
				action(element);
		}

		/// <summary>Dequeues an object from the queue and reenqueues it.</summary>
		/// <returns>Dequeued object.</returns>
		public static T Loop<T>(this Queue<T> queue)
		{
			T peek = queue.Dequeue();
			queue.Enqueue(peek);
			return peek;
		}

		#endregion GENERAL
	}
}