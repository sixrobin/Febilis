namespace RSLib.Extensions
{
	using UnityEngine;

    public static class CircleCollider2DExtensions
    {
        #region GENERAL

        /// <summary>
        /// Checks if two CircleCollider instances are overlapping.
        /// Transforms scales are not taken into account.
        /// </summary>
        /// <param name="other">Circle to check overlap with.</param>
        /// <returns>True if circles overlap, else false.</returns>
        public static bool OverlapsWith(this CircleCollider2D circle, CircleCollider2D other)
        {
            return (circle.radius + other.radius) * (circle.radius + other.radius) > (circle.transform.position - other.transform.position).sqrMagnitude;
        }

        #endregion
    }
}