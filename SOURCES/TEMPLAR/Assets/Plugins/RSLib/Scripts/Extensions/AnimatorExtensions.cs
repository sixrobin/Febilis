namespace RSLib.Extensions
{
    using UnityEngine;

    public static class AnimatorExtensions
    {
		#region GENERAL

		/// <summary>Checks if an animator has a given parameter.</summary>
		/// <param name="param">Parameter to look for.</param>
		/// <returns>True if the paramater exists.</returns>
		public static bool HasParameter(this Animator animator, string param)
		{
			if (string.IsNullOrEmpty(param))
				return false;

			for (int i = animator.parameters.Length - 1; i >= 0; --i)
				if (animator.parameters[i].name == param)
					return true;

			return false;
		}

        #endregion GENERAL
    }
}