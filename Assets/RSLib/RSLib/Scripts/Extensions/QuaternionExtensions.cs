namespace RSLib.Extensions
{
    using UnityEngine;

    public static class QuaternionExtensions
    {
        /// <summary>
        /// Inverses the sign of every component of a quaternion.
        /// </summary>
        /// <returns>A new quaternion with inversed signs.</returns>
        public static Quaternion InverseSigns(this Quaternion q)
        {
            return new Quaternion(-q.x, -q.y, -q.z, -q.w);
        }

        /// <summary>
        /// Checks if two quaternions are close to each other (dot being equal to 0f).
        /// Can be used to check if one of two which are supposed to be similar has its component signs reversed.
        /// </summary>
        /// <returns>True if two quaternions are close to each other.</returns>
        public static bool IsCloseTo(this Quaternion q1, Quaternion q2)
        {
            return Quaternion.Dot(q1, q2) >= 0f;
        }
    }
}