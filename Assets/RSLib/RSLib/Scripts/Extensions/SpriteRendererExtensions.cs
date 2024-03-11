namespace RSLib.Extensions
{
	using UnityEngine;

	public static class SpriteRendererExtensions
	{
		#region FLIP

		/// <summary>
		/// Flips the SpriteRenderer on the X axis.
		/// </summary>
		public static void FlipX(this SpriteRenderer spriteRenderer)
		{
			spriteRenderer.flipX = !spriteRenderer.flipX;
		}

		/// <summary>
		/// Flips the SpriteRenderer on the Y axis.
		/// </summary>
		public static void FlipY(this SpriteRenderer spriteRenderer)
		{
			spriteRenderer.flipY = !spriteRenderer.flipY;
		}

		#endregion // FLIP

		#region SET COLOR

		/// <summary>
		/// Sets the SpriteRenderer color.
		/// </summary>
		public static void SetColor(this SpriteRenderer spriteRenderer, Color color)
		{
			spriteRenderer.color = color;
		}

		/// <summary>
		/// Sets the SpriteRenderer color's red channel value.
		/// </summary>
		public static void SetColorRedValue(this SpriteRenderer spriteRenderer, float r)
		{
			Color color = spriteRenderer.color;
			color = new Color(r, color.g, color.b, color.a);
			spriteRenderer.color = color;
		}

		/// <summary>
		/// Sets the SpriteRenderer color's green channel value.
		/// </summary>
		public static void SetColorGreenValue(this SpriteRenderer spriteRenderer, float g)
		{
			Color color = spriteRenderer.color;
			color = new Color(color.r, g, color.b, color.a);
			spriteRenderer.color = color;
		}

		/// <summary>
		/// Sets the SpriteRenderer color's blue channel value.
		/// </summary>
		public static void SetColorBlueValue(this SpriteRenderer spriteRenderer, float b)
		{
			Color color = spriteRenderer.color;
			color = new Color(color.r, color.g, b, color.a);
			spriteRenderer.color = color;
		}

		/// <summary>
		/// Sets the SpriteRenderer color's alpha.
		/// </summary>
		public static void SetAlpha(this SpriteRenderer spriteRenderer, float a)
		{
			Color color = spriteRenderer.color;
			color = new Color(color.r, color.g, color.b, a);
			spriteRenderer.color = color;
		}

		#endregion // SET COLOR
	}
}