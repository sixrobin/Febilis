namespace RSLib.Maths
{
	using static System.Math;

	public enum Curve
	{
		Linear,
		InBack,
		InBounce,
		InCirc,
		InCubic,
		InElastic,
		InExpo,
		InQuad,
		InQuart,
		InQuint,
		InSine,
		OutBack,
		OutBounce,
		OutCirc,
		OutCubic,
		OutElastic,
		OutExpo,
		OutQuad,
		OutQuart,
		OutQuint,
		OutSine,
		InOutBack,
		InOutBounce,
		InOutCirc,
		InOutCubic,
		InOutElastic,
		InOutExpo,
		InOutQuad,
		InOutQuart,
		InOutQuint,
		InOutSine
	}

	/// <summary>
	/// Methods representing some easing curves where t is clamped between 0 and 1. Should be used to tweek the t parameter of Unity Lerp methods.
	/// Warning : some of the methods below will return a result below 0 or above 1, that will require Unity's LerpUnclamped.
	/// Curves visualizations : https://easings.net/fr.
	/// </summary>
	public static class Easing
	{
		public static float Ease(this float t, Curve c)
		{
			switch (c)
			{
				case Curve.InBack: return InBack(t);
				case Curve.InBounce: return InBounce(t);
				case Curve.InCirc: return InCirc(t);
				case Curve.InCubic: return InCubic(t);
				case Curve.InElastic: return InElastic(t);
				case Curve.InExpo: return InExpo(t);
				case Curve.InQuad: return InQuad(t);
				case Curve.InQuart: return InQuart(t);
				case Curve.InQuint: return InQuint(t);
				case Curve.InSine: return InSine(t);
				case Curve.OutBack: return OutBack(t);
				case Curve.OutBounce: return OutBounce(t);
				case Curve.OutCirc: return OutCirc(t);
				case Curve.OutCubic: return OutCubic(t);
				case Curve.OutElastic: return OutElastic(t);
				case Curve.OutExpo: return OutExpo(t);
				case Curve.OutQuad: return OutQuad(t);
				case Curve.OutQuart: return OutQuart(t);
				case Curve.OutQuint: return OutQuint(t);
				case Curve.OutSine: return OutSine(t);
				case Curve.InOutBack: return InOutBack(t);
				case Curve.InOutBounce: return InOutBounce(t);
				case Curve.InOutCirc: return InOutCirc(t);
				case Curve.InOutCubic: return InOutCubic(t);
				case Curve.InOutElastic: return InOutElastic(t);
				case Curve.InOutExpo: return InOutExpo(t);
				case Curve.InOutQuad: return InOutQuad(t);
				case Curve.InOutQuart: return InOutQuart(t);
				case Curve.InOutQuint: return InOutQuint(t);
				case Curve.InOutSine: return InOutSine(t);

				case Curve.Linear:
				default: return t;
			}
		}

		#region EASING FUNCTIONS

		public static float InBack(this float t)
		{
			float s = 1.70158f;
			return t * t * ((s + 1f) * t - s);
		}

		public static float OutBack(this float t)
		{
			float s = 1.70158f;
			return 1f + (t - 1f) * (t - 1f) * ((s + 1f) * (t - 1f) + s);
		}

		public static float InOutBack(this float t)
		{
			float s = 1.70158f * 1.525f;
			return t < 0.5f
				? 0.5f * (4f * t * t * ((s + 1f) * (t * 2f) - s))
				: 0.5f * ((t * 2f - 2f) * (t * 2f - 2f) * ((s + 1f) * (t * 2f - 2f) + s) + 2f);
		}

		public static float InBounce(this float t)
		{
			return 1 - OutBounce(1 - t);
		}

		public static float OutBounce(this float t)
		{
			if (t < 1f / 2.75f)
				return 7.5625f * t * t;

			if (t < 2f / 2.75f)
				return 7.5625f * (t - 1.5f / 2.75f) * (t - 1.5f / 2.75f) + 0.75f;

			if (t < 2.5f / 2.75f)
				return 7.5625f * (t - 2.25f / 2.75f) * (t - 2.25f / 2.75f) + 0.9375f;

			return 7.5625f * (t - 2.625f / 2.75f) * (t - 2.625f / 2.75f) + 0.984375f;
		}

		public static float InOutBounce(this float t)
		{
			return t < 0.5f
				? InBounce(t * 2f) * 0.5f
				: OutBounce(t * 2f - 1f) * 0.5f + 0.5f;
		}

		public static float InCirc(this float t)
		{
			return -(float)(Sqrt(1f - t * t) - 1f);
		}

		public static float OutCirc(this float t)
		{
			return (float)Sqrt(1f - (t * t - 2f * t + 1f));
		}

		public static float InOutCirc(this float t)
		{
			return t < 0.5f
				? -0.5f * ((float)Sqrt(1f - 4f * t * t) - 1f)
				: 0.5f * ((float)Sqrt(1f - (4f * t * t - 8f * t + 4f)) + 1f);
		}

		public static float InCubic(this float t)
		{
			return t * t * t;
		}

		public static float OutCubic(this float t)
		{
			return --t * t * t + 1f;
		}

		public static float InOutCubic(this float t)
		{
			return t < 0.5f
				? 4f * t * t * t
				: (t - 1f) * (4f * t * t - 8f * t + 4f) + 1f;
		}

		public static float InElastic(this float t)
		{
			if (t == 0f || t == 1f)
				return t;

			float f = 0.3f;
			float s = f * 0.25f;

			return -(float)(Pow(2f, 10f * (t -= 1f)) * Sin((t * 1f - s) * (2f * PI) / f));
		}

		public static float OutElastic(this float t)
		{
			if (t == 0f || t == 1f)
				return t;

			float f = 0.3f;
			float s = f * 0.25f;

			return (float)(Pow(2f, -10f * t) * Sin((t - s) * 2f * PI / f) + 1f);
		}

		public static float InOutElastic(this float t)
		{
			if (t == 0f || (t /= 0.5f) == 2f)
				return t;

			float f = 0.3f;
			float s = f * 0.25f;

			return t < 1f
				? -0.5f * (float)(Pow(2f, 10f * --t) * Sin((t - s) * 2f * PI / f))
				: (float)(Pow(2f, -10f * --t) * Sin((t - s) * 2f * PI / f) * 0.5f + 1f);
		}

		public static float InExpo(this float t)
		{
			return (float)Pow(2f, 10f * (t - 1f));
		}

		public static float OutExpo(this float t)
		{
			return -(float)Pow(2f, -10f * t) + 1f;
		}

		public static float InOutExpo(this float t)
		{
			return t < 0.5f
				? 0.5f * (float)Pow(2f, 10f * (t * 2f - 1f))
				: 0.5f * (-(float)Pow(2f, -10f * (t * 2f - 1f)) + 2f);
		}

		public static float InQuad(this float t)
		{
			return t * t;
		}

		public static float OutQuad(this float t)
		{
			return t * (2f - t);
		}

		public static float InOutQuad(this float t)
		{
			return t < 0.5f
				? 2f * t * t
				: (4f - 2f * t) * t - 1f;
		}

		public static float InQuart(this float t)
		{
			return t * t * t * t;
		}

		public static float OutQuart(this float t)
		{
			return 1f - (--t) * t * t * t;
		}

		public static float InOutQuart(this float t)
		{
			return t < 0.5f
				? 8f * t * t * t * t
				: 1f - 8f * --t * t * t * t;
		}

		public static float InQuint(this float t)
		{
			return t * t * t * t * t;
		}

		public static float OutQuint(this float t)
		{
			return --t * t * t * t * t + 1f;
		}

		public static float InOutQuint(this float t)
		{
			return t < 0.5f
				? 16f * t * t * t * t * t
				: 16f * --t * t * t * t * t + 1f;
		}

		public static float InSine(this float t)
		{
			return -(float)Cos(t * (PI / 2f)) + 1f;
		}

		public static float OutSine(this float t)
		{
			return (float)Sin(t * (PI / 2f));
		}

		public static float InOutSine(this float t)
		{
			return -0.5f * (float)(Cos(PI * t) - 1f);
		}

		#endregion EASING FUNCTIONS
	}
}