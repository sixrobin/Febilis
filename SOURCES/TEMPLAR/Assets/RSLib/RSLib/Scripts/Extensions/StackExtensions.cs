namespace RSLib.Extensions
{
	using System.Collections.Generic;

	public static class StackExtensions
	{
		#region CONVERSION

		/// <summary>Writes the stack elements in a string.</summary>
		/// <param name="stack">Stack to write.</param>
		/// <returns>String with the stack elements.</returns>
		public static string ToStringImproved<T>(this Stack<T> stack)
		{
			string str = string.Empty;
			int elementIndex = 0;

			foreach (T element in stack)
			{
				++elementIndex;
				str += element.ToString() + (elementIndex == stack.Count ? "" : ", ");
			}

			return str;
		}

		/// <summary>Writes the stack elements in a string using a given splitting char.</summary>
		/// <param name="stack">Stack to write.</param>
		/// <param name="split">Splitting char.</param>
		/// <returns>String with the stack elements.</returns>
		public static string ToStringImproved<T>(this Stack<T> stack, char split)
		{
			string str = string.Empty;
			int elementIndex = 0;

			foreach (T element in stack)
			{
				++elementIndex;
				str += element.ToString() + (elementIndex == stack.Count ? "" : split.ToString());
			}

			return str;
		}

		/// <summary>Writes the stack elements in a string using a given splitting string.</summary>
		/// <param name="stack">Stack to write.</param>
		/// <param name="split">Splitting string.</param>
		/// <returns>String with the stack elements.</returns>
		public static string ToStringImproved<T>(this Stack<T> stack, string split)
		{
			string str = string.Empty;
			int elementIndex = 0;

			foreach (T element in stack)
			{
				++elementIndex;
				str += element.ToString() + (elementIndex == stack.Count ? "" : string.IsNullOrEmpty(split) ? " / " : split);
			}

			return str;
		}

		#endregion CONVERSION

		#region GENERAL

		/// <summary>Loops through all elements in the stack and executes an action.</summary>
		/// <param name="action">Action to execute.</param>
		public static void ForEach<T>(this Stack<T> stack, System.Action<T> action)
		{
			foreach (T element in stack)
				action(element);
		}

		/// <summary>Pushes all the elements of an IEnumerable.</summary>
		/// <param name="collection">The IEnumerable to push elements from.</param>
		public static void Push<T>(this Stack<T> stack, IEnumerable<T> collection)
		{
			foreach (T element in collection)
				stack.Push(element);
		}

		#endregion GENERAL
	}
}