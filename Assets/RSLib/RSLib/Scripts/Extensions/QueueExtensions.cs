namespace RSLib.Extensions
{
	using System.Collections.Generic;

	public static class QueueExtensions
    {
		#region GENERAL

	    /// <summary>
	    /// Loops through all elements in the queue and executes an action.
	    /// </summary>
	    /// <param name="queue">Queue to loop through.</param>
	    /// <param name="action">Action to execute.</param>
	    public static void ForEach<T>(this Queue<T> queue, System.Action<T> action)
	    {
		    foreach (T element in queue)
			    action(element);
	    }
	    
		/// <summary>
		/// Enqueues all the elements of an IEnumerable.
		/// </summary>
		/// <param name="queue">Queue to enqueue elements into.</param>
		/// <param name="collection">The IEnumerable to enqueue elements from.</param>
		public static void Enqueue<T>(this Queue<T> queue, IEnumerable<T> collection)
		{
			foreach (T element in collection)
				queue.Enqueue(element);
		}

		/// <summary>
		/// Dequeues an object from the queue and re-enqueues it.
		/// </summary>
		/// <returns>Dequeued object.</returns>
		public static T Loop<T>(this Queue<T> queue)
		{
			T peek = queue.Dequeue();
			queue.Enqueue(peek);
			return peek;
		}

		#endregion // GENERAL
	}
}