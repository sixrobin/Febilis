namespace RSLib.Extensions
{
	using UnityEngine;

    public static class BoxCollider2DExtensions
    {
		private static Transform s_cachedTransform = null;

		public enum BoxSide
		{
			LEFT,
			RIGHT,
			TOP,
			BOTTOM
		}

        #region CORNERS

        static Vector2[] Corners(this BoxCollider2D box, BoxSide side)
		{
            switch (side)
			{
				case BoxSide.LEFT: return CornersLeft(box);
				case BoxSide.RIGHT: return CornersRight(box);
				case BoxSide.TOP: return CornersTop(box);
				case BoxSide.BOTTOM: return CornersBottom(box);
				default: return null;
			}
		}

		/// <summary>
		/// Gets all corners of a BoxCollider2D, without considering parents transforms.
		/// </summary>
		/// <returns>Array of four Vector2, starting from bottom left going clockwise.</returns>
		public static Vector2[] Corners(this BoxCollider2D box)
		{
			Vector2[] corners = new Vector2[4];
			s_cachedTransform = box.transform;

			Vector2 position = s_cachedTransform.position;
			Quaternion rotation = s_cachedTransform.localRotation;
			Vector2 scale = s_cachedTransform.localScale;

			Vector2 diagonal = Vector2.Scale(box.size, scale) * 0.5f;
			Vector2 angledDiagonal = rotation * diagonal;
			Vector2 angledDiagonalOpposite = rotation * new Vector3(-diagonal.x, diagonal.y);

			position += (Vector2)(rotation * Vector2.Scale(box.offset, scale));

			corners[0] += position + new Vector2(-angledDiagonal.x, -angledDiagonal.y);
			corners[1] += position + new Vector2(+angledDiagonalOpposite.x, +angledDiagonalOpposite.y);
			corners[2] += position + new Vector2(+angledDiagonal.x, +angledDiagonal.y);
			corners[3] += position + new Vector2(-angledDiagonalOpposite.x, -angledDiagonalOpposite.y);

			return corners;
		}

		/// <summary>
		/// Gets bottom corners of a BoxCollider2D, without considering parents transforms.
		/// </summary>
		/// <returns>Array of two Vector2, bottom left and bottom right.</returns>
		public static Vector2[] CornersBottom(this BoxCollider2D box)
		{
			Vector2[] corners = new Vector2[2];
			s_cachedTransform = box.transform;

			Vector2 position = s_cachedTransform.position;
			Quaternion rotation = s_cachedTransform.localRotation;
			Vector2 scale = s_cachedTransform.localScale;

			Vector2 diagonal = Vector2.Scale(box.size, scale) * 0.5f;
			Vector2 angledDiagonal = rotation * diagonal;
			Vector2 angledDiagonalOpposite = rotation * new Vector3(-diagonal.x, diagonal.y);

			position += (Vector2)(rotation * Vector2.Scale(box.offset, scale));

			corners[0] += position + new Vector2(-angledDiagonal.x, -angledDiagonal.y);
			corners[1] += position + new Vector2(-angledDiagonalOpposite.x, -angledDiagonalOpposite.y);

			return corners;
		}

		/// <summary>
		/// Gets left corners of a BoxCollider2D, without considering parents transforms.
		/// </summary>
		/// <returns>Array of two Vector2, bottom left and top left.</returns>
		public static Vector2[] CornersLeft(this BoxCollider2D box)
		{
			Vector2[] corners = new Vector2[2];
			s_cachedTransform = box.transform;

			Vector2 position = s_cachedTransform.position;
			Quaternion rotation = s_cachedTransform.localRotation;
			Vector2 scale = s_cachedTransform.localScale;

			Vector2 diagonal = Vector2.Scale(box.size, scale) * 0.5f;
			Vector2 angledDiagonal = rotation * diagonal;
			Vector2 angledDiagonalOpposite = rotation * new Vector3(-diagonal.x, diagonal.y);

			position += (Vector2)(rotation * Vector2.Scale(box.offset, scale));

			corners[0] += position + new Vector2(-angledDiagonal.x, -angledDiagonal.y);
			corners[1] += position + new Vector2(+angledDiagonalOpposite.x, +angledDiagonalOpposite.y);

			return corners;
		}

		/// <summary>
		/// Gets right corners of a BoxCollider2D, without considering parents transforms.
		/// </summary>
		/// <returns>Array of two Vector2, bottom right and top right.</returns>
		public static Vector2[] CornersRight(this BoxCollider2D box)
		{
			Vector2[] corners = new Vector2[2];
			s_cachedTransform = box.transform;

			Vector2 position = s_cachedTransform.position;
			Quaternion rotation = s_cachedTransform.localRotation;
			Vector2 scale = s_cachedTransform.localScale;

			Vector2 diagonal = Vector2.Scale(box.size, scale) * 0.5f;
			Vector2 angledDiagonal = rotation * diagonal;
			Vector2 angledDiagonalOpposite = rotation * new Vector3(-diagonal.x, diagonal.y);

			position += (Vector2)(rotation * Vector2.Scale(box.offset, scale));

			corners[0] += position + new Vector2(-angledDiagonalOpposite.x, -angledDiagonalOpposite.y);
			corners[1] += position + new Vector2(+angledDiagonal.x, +angledDiagonal.y);

			return corners;
		}

		/// <summary>
		/// Gets top corners of a BoxCollider2D, without considering parents transforms.
		/// </summary>
		/// <returns>Array of two Vector2, top left and top right.</returns>
		public static Vector2[] CornersTop(this BoxCollider2D box)
		{
			Vector2[] corners = new Vector2[2];
			s_cachedTransform = box.transform;

			Vector2 position = s_cachedTransform.position;
			Quaternion rotation = s_cachedTransform.localRotation;
			Vector2 scale = s_cachedTransform.localScale;

			Vector2 diagonal = Vector2.Scale(box.size, scale) * 0.5f;
			Vector2 angledDiagonal = rotation * diagonal;
			Vector2 angledDiagonalOpposite = rotation * new Vector3(-diagonal.x, diagonal.y);

			position += (Vector2)(rotation * Vector2.Scale(box.offset, scale));

			corners[0] += position + new Vector2(+angledDiagonalOpposite.x, +angledDiagonalOpposite.y);
			corners[1] += position + new Vector2(+angledDiagonal.x, +angledDiagonal.y);

			return corners;
		}

        #endregion // CORNERS

        #region GENERAL

        /// <summary>
		/// Gets points alongside a BoxCollider2D side.
		/// </summary>
        /// <param name="side">Side used for calculations.</param>
        /// <param name="count">Numbers of points.</param>
        /// <returns>Points alongside a side. Returns corners if count is less or equal to 2.</returns>
        public static Vector2[] PointsAlongSide(this BoxCollider2D box, BoxSide side, int count)
		{
			if (count <= 2)
				return box.Corners(side);

			Vector2[] points = new Vector2[count];
			Vector2[] corners = box.Corners(side);
			Vector2 start = corners[0];
			Vector2 end = corners[1];

			count--;
			float pointsCount = count;

			for (int i = 0; i <= count; ++i)
				points[i] = start * (1 - i / pointsCount) + end * (i / pointsCount);

			return points;
		}

        /// <summary>
		/// Checks if two BoxCollider2D instances are overlapping.
		/// </summary>
        /// <param name="other">Box to check overlap with.</param>
        /// <returns>True if boxes overlap, else false.</returns>
        public static bool OverlapsWith(this BoxCollider2D box, BoxCollider2D other)
        {
            return !(box.bounds.min.x > other.bounds.max.x
                || box.bounds.max.x < other.bounds.min.x
                || box.bounds.min.y > other.bounds.max.y
                || box.bounds.max.y < other.bounds.min.y);
        }

        /// <summary>
		/// Computes a random position inside a BoxCollider2D.
		/// Does not take rotation into account.
		/// </summary>
        /// <returns>A random point inside the box bounds.</returns>
        public static Vector2 RandomPointInside(this BoxCollider2D box)
        {
            return new Vector2(Random.Range(box.bounds.min.x, box.bounds.max.x), Random.Range(box.bounds.min.y, box.bounds.max.y));
        }

		#endregion // GENERAL
	}
}