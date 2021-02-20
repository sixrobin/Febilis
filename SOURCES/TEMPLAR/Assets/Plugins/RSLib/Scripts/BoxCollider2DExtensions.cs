namespace RSLib.Extensions
{
	using UnityEngine;

    public static class BoxCollider2DExtensions
    {
		private static Transform s_cachedTransform = null;

		public enum BoxSide
		{
			Left,
			Right,
			Top,
			Bottom
		}

        #region CORNERS

        static Vector2[] Corners(this BoxCollider2D box, BoxSide side)
		{
			switch (side)
			{
				case BoxSide.Left: return CornersLeft(box);
				case BoxSide.Right: return CornersRight(box);
				case BoxSide.Top: return CornersTop(box);
				case BoxSide.Bottom: return CornersBottom(box);
				default: return null;
			}
		}

		/// <summary>Gets all corners of a BoxCollider2D, without considering parents transforms.</summary>
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
			Vector2 angledDiagonalOpposite = rotation * diagonal.WithX(-diagonal.x);

			position += (Vector2)(rotation * Vector2.Scale(box.offset, scale));

			corners[0] += position + new Vector2(-angledDiagonal.x, -angledDiagonal.y);
			corners[1] += position + new Vector2(+angledDiagonalOpposite.x, +angledDiagonalOpposite.y);
			corners[2] += position + new Vector2(+angledDiagonal.x, +angledDiagonal.y);
			corners[3] += position + new Vector2(-angledDiagonalOpposite.x, -angledDiagonalOpposite.y);

			return corners;
		}

		/// <summary>Gets bottom corners of a BoxCollider2D, without considering parents transforms.</summary>
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
			Vector2 angledDiagonalOpposite = rotation * diagonal.WithX(-diagonal.x);

			position += (Vector2)(rotation * Vector2.Scale(box.offset, scale));

			corners[0] += position + new Vector2(-angledDiagonal.x, -angledDiagonal.y);
			corners[1] += position + new Vector2(-angledDiagonalOpposite.x, -angledDiagonalOpposite.y);

			return corners;
		}

		/// <summary>Gets left corners of a BoxCollider2D, without considering parents transforms.</summary>
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
			Vector2 angledDiagonalOpposite = rotation * diagonal.WithX(-diagonal.x);

			position += (Vector2)(rotation * Vector2.Scale(box.offset, scale));

			corners[0] += position + new Vector2(-angledDiagonal.x, -angledDiagonal.y);
			corners[1] += position + new Vector2(+angledDiagonalOpposite.x, +angledDiagonalOpposite.y);

			return corners;
		}

		/// <summary>Gets right corners of a BoxCollider2D, without considering parents transforms.</summary>
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
			Vector2 angledDiagonalOpposite = rotation * diagonal.WithX(-diagonal.x);

			position += (Vector2)(rotation * Vector2.Scale(box.offset, scale));

			corners[0] += position + new Vector2(-angledDiagonalOpposite.x, -angledDiagonalOpposite.y);
			corners[1] += position + new Vector2(+angledDiagonal.x, +angledDiagonal.y);

			return corners;
		}

		/// <summary>Gets top corners of a BoxCollider2D, without considering parents transforms.</summary>
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
			Vector2 angledDiagonalOpposite = rotation * diagonal.WithX(-diagonal.x);

			position += (Vector2)(rotation * Vector2.Scale(box.offset, scale));

			corners[0] += position + new Vector2(+angledDiagonalOpposite.x, +angledDiagonalOpposite.y);
			corners[1] += position + new Vector2(+angledDiagonal.x, +angledDiagonal.y);

			return corners;
		}

        #endregion CORNERS

        #region GENERAL

        /// <summary>Gets points alongside a BoxCollider2D side.</summary>
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

		#endregion GENERAL
	}
}