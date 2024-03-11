namespace RSLib.Extensions
{
    using UnityEngine;

    public static class RigidbodyExtensions
    {
		#region RIGIDBODY 2D

		/// <summary>
		/// Changes the direction of a rigidbody, keeping its velocity.
		/// </summary>
		/// <param name="rb">Rigidbody to change the direction of.</param>
		/// <param name="dir">New direction.</param>
		public static void ChangeDirection(this Rigidbody2D rb, Vector2 dir)
		{
			rb.velocity = dir * rb.velocity.magnitude;
		}

		/// <summary>
		/// Freezes the rigidbody and sets it as kinematic.
		/// <param name="rb">Rigidbody to freeze.</param>
		/// </summary>
		public static void Freeze(this Rigidbody2D rb)
		{
			rb.velocity *= 0f;
			rb.angularVelocity *= 0f;
			rb.isKinematic = true;
		}

		/// <summary>
		/// Nullifies the rigidbody motion (velocity and angular velocity).
		/// <param name="rb">Rigidbody to nullify movement of.</param>
		/// </summary>
		public static void NullifyMovement(this Rigidbody2D rb)
		{
			rb.velocity *= 0f;
			rb.angularVelocity *= 0f;
		}

		#endregion // RIGIDBODY 2D

		#region RIGIDBODY 3D

		/// <summary>
		/// Changes the direction of a rigidbody, keeping its velocity.
		/// </summary>
		/// <param name="rb">Rigidbody to change the direction of.</param>
		/// <param name="dir">New direction.</param>
		public static void ChangeDirection(this Rigidbody rb, Vector3 dir)
		{
			rb.velocity = dir * rb.velocity.magnitude;
		}

		/// <summary>
		/// Freezes the rigidbody and sets it as kinematic.
		/// <param name="rb">Rigidbody to freeze.</param>
		/// </summary>
		public static void Freeze(this Rigidbody rb)
		{
			rb.velocity *= 0f;
			rb.angularVelocity *= 0f;
			rb.isKinematic = true;
		}

		/// <summary>
		/// Nullifies the rigidbody motion (velocity and angular velocity).
		/// <param name="rb">Rigidbody to nullify movement of.</param>
		/// </summary>
		public static void NullifyMovement(this Rigidbody rb)
		{
			rb.velocity *= 0f;
			rb.angularVelocity *= 0f;
		}

		#endregion // RIGIDBODY 3D
	}
}