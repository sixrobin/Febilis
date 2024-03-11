namespace RSLib.Extensions
{
	using System.Collections.Generic;

	public static class StackExtensions
	{
		#region GENERAL

		/// <summary>
		/// Loops through all elements in the stack and executes an action.
		/// </summary>
		/// <param name="stack">Stack to loop through.</param>
		/// <param name="action">Action to execute.</param>
		public static void ForEach<T>(this Stack<T> stack, System.Action<T> action)
		{
			foreach (T element in stack)
				action(element);
		}

		/// <summary>
		/// Pushes all the elements of an IEnumerable.
		/// </summary>
		/// <param name="stack">Stack to push elements into.</param>
		/// <param name="collection">The IEnumerable to push elements from.</param>
		public static void Push<T>(this Stack<T> stack, IEnumerable<T> collection)
		{
			foreach (T element in collection)
				stack.Push(element);
		}

		#endregion // GENERAL
	}
}