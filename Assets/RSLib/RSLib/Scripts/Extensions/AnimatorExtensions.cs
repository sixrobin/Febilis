namespace RSLib.Extensions
{
    using UnityEngine;

    public static class AnimatorExtensions
    {
		#region GENERAL

		/// <summary>
		/// Checks if an animator has a given parameter.
		/// </summary>
		/// <param name="animator">Animator to search in.</param>
		/// <param name="param">Parameter to look for. Cannot be null or empty.</param>
		/// <returns>True if the parameter exists.</returns>
		public static bool HasParameter(this Animator animator, string param)
		{
			if (string.IsNullOrEmpty(param))
				throw new System.Exception("Animator parameter cannot be null or empty.");

			for (int i = animator.parameters.Length - 1; i >= 0; --i)
				if (animator.parameters[i].name == param)
					return true;

			return false;
		}

        #endregion // GENERAL
    }
}