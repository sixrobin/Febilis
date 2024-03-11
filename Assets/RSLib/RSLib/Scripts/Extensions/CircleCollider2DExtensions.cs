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
        /// <param name="circle">First circle to check overlap with.</param>
        /// <param name="otherCircle">Second circle to check overlap with.</param>
        /// <returns>True if circles overlap, else false.</returns>
        public static bool OverlapsWith(this CircleCollider2D circle, CircleCollider2D otherCircle)
        {
            float circleRadius = circle.radius;
            float otherCircleRadius = otherCircle.radius;
            return (circleRadius + otherCircleRadius) * (circleRadius + otherCircleRadius) > (circle.transform.position - otherCircle.transform.position).sqrMagnitude;
        }

        #endregion // GENERAL
    }
}