namespace RSLib.Extensions
{
	using System.Linq;
    using UnityEngine;

    public static class TransformExtensions
    {
		#region ADD

		/// <summary>
		/// Increments all components of the transform position.
		/// </summary>
		/// <param name="value">Incrementation amount.</param>
		public static void AddPositionAll(this Transform t, float value)
		{
			t.position += Vector3.one * value;
		}

		/// <summary>
		/// Increments the x component of the transform position.
		/// </summary>
		/// <param name="value">Incrementation amount.</param>
		public static void AddPositionX(this Transform t, float value)
		{
			t.position += new Vector3(value, 0f, 0f);
		}

		/// <summary>
		/// Increments the y component of the transform position.
		/// </summary>
		/// <param name="value">Incrementation amount.</param>
		public static void AddPositionY(this Transform t, float value)
		{
			t.position += new Vector3(0f, value, 0f);
		}

		/// <summary>
		/// Increments the z component of the transform position.
		/// </summary>
		/// <param name="value">Incrementation amount.</param>
		public static void AddPositionZ(this Transform t, float value)
		{
			t.position += new Vector3(0f, 0f, value);
		}

		#endregion // ADD

		#region GENERAL

		/// <summary>
		/// Destroys all transform children.
		/// </summary>
		public static void DestroyChildren(this Transform t)
		{
			for (int i = t.childCount - 1; i >= 0; --i)
				Object.Destroy(t.GetChild(i).gameObject);

			t.DetachChildren();
		}

		/// <summary>
		/// Destroys immediate all transform children.
		/// </summary>
		public static void DestroyImmediateChildren(this Transform t)
		{
			for (int i = t.childCount - 1; i >= 0; --i)
				Object.DestroyImmediate(t.GetChild(i).gameObject);

			t.DetachChildren();
		}

		/// <summary>
		/// Transfers all children of a transform to another parent.
		/// </summary>
		/// <param name="newParent">New parent transform.</param>
		public static void TransferChildren(this Transform t, Transform newParent)
		{
			Transform[] children = new Transform[t.childCount];
			for (int i = children.Length - 1; i >= 0; --i)
				children[i] = t.GetChild(i);

			for (int i = children.Length - 1; i >= 0; --i)
			{
				children[i].SetParent(newParent);
				children[i].SetAsFirstSibling();
			}
		}

		#endregion // GENERAL

		#region GET CLOSEST

		/// <summary>
		/// Gets the closest transform among a collection of compared ones.
		/// </summary>
		/// <param name="compared">Compared transforms.</param>
		/// <param name="avoidSelf">Avoid the reference transform.</param>
		/// <returns>Closest transform found.</returns>
		public static Transform GetClosestTransform(this Transform t, System.Collections.Generic.IEnumerable<Transform> compared, bool avoidSelf = true)
		{
			Transform closest = null;
			float sqrClosestDist = Mathf.Infinity;

			foreach (Transform target in compared)
			{
				if (avoidSelf && target == t)
					continue;

				float sqrTargetDist = (t.position - target.position).sqrMagnitude;
				if (sqrTargetDist > sqrClosestDist)
					continue;

				sqrClosestDist = sqrTargetDist;
				closest = target;
			}

			return closest;
		}

		/// <summary>
		/// Gets the closest transforms among a collection of compared ones.
		/// </summary>
		/// <param name="compared">Compared transforms.</param>
		/// <param name="quantity">Amount of transforms desired.</param>
		/// <param name="avoidSelf">Avoid the reference transform.</param>
		/// <returns>Closest transforms found.</returns>
		public static Transform[] GetClosestTransforms(this Transform t, System.Collections.Generic.IEnumerable<Transform> compared, int quantity, bool avoidSelf = true)
		{
			System.Collections.Generic.List<Transform> comparedToList = (compared as Transform[]).ToList();
			if (avoidSelf && comparedToList.Contains(t))
				comparedToList.Remove(t);

			comparedToList.Sort((a, b) => Vector3.SqrMagnitude(t.position - a.position).CompareTo(Vector3.SqrMagnitude(t.position - b.position)));

			int quantityClamped = Mathf.Clamp(quantity, 0, comparedToList.Count);
			Transform[] closestTransforms = new Transform[quantityClamped];

			for (int i = quantityClamped - 1; i >= 0; --i)
				closestTransforms[i] = comparedToList[i];

			return closestTransforms;
		}

		#endregion // GET CLOSEST

		#region RESET

		/// <summary>
		/// Resets the transform overall component.
		/// </summary>
		public static void ResetAll(this Transform t, bool useLocalPosition = false)
		{
			if (useLocalPosition)
				t.ResetLocalPosition();
			else
				t.ResetPosition();

			t.ResetRotation();
			t.ResetScale();
		}

		/// <summary>
		/// Resets the transform position.
		/// </summary>
		public static void ResetPosition(this Transform t)
		{
			t.position = Vector3.zero;
		}

		/// <summary>
		/// Resets the transform local position.
		/// </summary>
		public static void ResetLocalPosition(this Transform t)
		{
			t.localPosition = Vector3.zero;
		}

		/// <summary>
		/// Resets the transform rotation.
		/// </summary>
		public static void ResetRotation(this Transform t)
		{
			t.rotation = Quaternion.identity;
		}

		/// <summary>
		/// Resets the transform local rotation.
		/// </summary>
		public static void ResetLocalRotation(this Transform t)
		{
			t.localRotation = Quaternion.identity;
		}

		/// <summary>
		/// Resets the transform local scale.
		/// </summary>
		public static void ResetScale(this Transform t)
		{
			t.localScale = Vector3.one;
		}

		/// <summary>
		/// Resets the children transforms positions.
		/// </summary>
		/// <param name="recursive">Children also reset their own children.</param>
		public static void ResetChildrenLocalPositions(this Transform t, bool recursive = false)
		{
			foreach (Transform child in t)
			{
				child.ResetLocalPosition();
				if (recursive)
					child.ResetChildrenLocalPositions(true);
			}
		}

		/// <summary>
		/// Resets the children transforms rotations.
		/// </summary>
		/// <param name="recursive">Children also reset their own children.</param>
		public static void ResetChildrenLocalRotations(this Transform t, bool recursive = false)
		{
			foreach (Transform child in t)
			{
				child.ResetRotation();
				if (recursive)
					child.ResetChildrenLocalRotations(true);
			}
		}

		/// <summary>
		/// Resets the children transforms local scales.
		/// </summary>
		/// <param name="recursive">Children also reset their own children.</param>
		public static void ResetChildrenScales(this Transform t, bool recursive = false)
		{
			foreach (Transform child in t)
			{
				child.ResetScale();
				if (recursive)
					child.ResetChildrenScales(true);
			}
		}

		/// <summary>
		/// Resets the children all transforms components.
		/// </summary>
		/// <param name="recursive">Children also reset their own children.</param>
		public static void ResetChildrenAll(this Transform t, bool recursive = false)
		{
			foreach (Transform child in t)
			{
				child.ResetAll(true);
				if (recursive)
					child.ResetChildrenAll(true);
			}
		}

		#endregion // RESET

		#region SET

		/// <summary>
		/// Sets the x component of the transform position.
		/// </summary>
		/// <param name="value">New x value.</param>
		public static void SetPositionX(this Transform t, float value)
		{
			t.position = new Vector3(value, t.position.y, t.position.z);
		}

		/// <summary>
		/// Sets the y component of the transform position.
		/// </summary>
		/// <param name="y">New y value.</param>
		public static void SetPositionY(this Transform t, float value)
		{
			t.position = new Vector3(t.position.x, value, t.position.z);
		}

		/// <summary>
		/// Sets the z component of the transform position.
		/// </summary>
		/// <param name="value">New z value.</param>
		public static void SetPositionZ(this Transform t, float value)
		{
			t.position = new Vector3(t.position.x, t.position.y, value);
		}

		/// <summary>
		/// Sets the x component of the transform local position.
		/// </summary>
		/// <param name="value">New local x value.</param>
		public static void SetLocalPositionX(this Transform t, float value)
		{
			t.localPosition = new Vector3(value, t.localPosition.y, t.localPosition.z);
		}

		/// <summary>
		/// Sets the y component of the transform local position.
		/// </summary>
		/// <param name="y">New local y value.</param>
		public static void SetLocalPositionY(this Transform t, float value)
		{
			t.localPosition = new Vector3(t.localPosition.x, value, t.localPosition.z);
		}

		/// <summary>
		/// Sets the z component of the transform local position.
		/// </summary>
		/// <param name="value">New local z value.</param>
		public static void SetLocalPositionZ(this Transform t, float value)
		{
			t.localPosition = new Vector3(t.localPosition.x, t.localPosition.y, value);
		}

		/// <summary>
		/// Sets the x component of the transform local eulerAngles.
		/// </summary>
		/// <param name="value">New x value.</param>
		public static void SetEulerAnglesX(this Transform t, float value)
		{
			t.localEulerAngles = new Vector3(value, t.localEulerAngles.y, t.localEulerAngles.z);
		}

		/// <summary>
		/// Sets the y component of the transform local eulerAngles.
		/// </summary>
		/// <param name="value">New y value.</param>
		public static void SetEulerAnglesY(this Transform t, float value)
		{
			t.localEulerAngles = new Vector3(t.localEulerAngles.x, value, t.localEulerAngles.z);
		}

		/// <summary>
		/// Sets the z component of the transform local eulerAngles.
		/// </summary>
		/// <param name="value">New z value.</param>
		public static void SetEulerAnglesZ(this Transform t, float value)
		{
			t.localEulerAngles = new Vector3(t.localEulerAngles.x, t.localEulerAngles.y, value);
		}

		/// <summary>
		/// Sets all components of the transform local scale.
		/// </summary>
		/// <param name="value">New scale.</param>
		public static void SetScale(this Transform t, float value)
		{
			t.localScale = Vector3.one * value;
		}

		/// <summary>
		/// Sets the x component of the transform local scale.
		/// </summary>
		/// <param name="value">New x value.</param>
		public static void SetScaleX(this Transform t, float value)
		{
			t.localScale = new Vector3(value, t.localScale.y, t.localScale.z);
		}

		/// <summary>
		/// Sets the y component of the transform local scale.
		/// </summary>
		/// <param name="value">New y value.</param>
		public static void SetScaleY(this Transform t, float value)
		{
			t.localScale = new Vector3(t.localScale.x, value, t.localScale.z);
		}

		/// <summary>
		/// Sets the z component of the transform local scale.
		/// </summary>
		/// <param name="value">New z value.</param>
		public static void SetScaleZ(this Transform t, float value)
		{
			t.localScale = new Vector3(t.localScale.x, t.localScale.y, value);
		}

		#endregion // SET
	}
}