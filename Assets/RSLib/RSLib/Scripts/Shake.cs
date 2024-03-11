namespace RSLib
{
	using UnityEngine;

	[System.Flags]
	public enum CoordinateAxes : byte
	{
		X = 1,
		Y = 2,
		Z = 4,
		XY = X | Y,
		XZ = X | Z,
		YZ = Y | Z,
		XYZ = X | Y | Z
	}

	public class Shake
	{
		[System.Serializable]
		public struct ShakeSettings
		{
			public CoordinateAxes PosAxes;
			public CoordinateAxes RotAxes;
			public float Speed;
			public float Radius;
			public float XRotMax;
			public float YRotMax;
			public float ZRotMax;

            public static ShakeSettings Default => new ShakeSettings()
            {
                PosAxes = CoordinateAxes.XY,
                RotAxes = CoordinateAxes.XYZ,
                Speed = 15,
                Radius = 0.3f,
                XRotMax = 15,
                YRotMax = 15,
                ZRotMax = 15
            };
        }

		public Shake()
		{
			Settings = ShakeSettings.Default;
		}

		public Shake(ShakeSettings settings)
		{
			Settings = settings;
		}

		private float _trauma;
		public float Trauma
        {
            get => _trauma;
            private set => _trauma = Mathf.Clamp01(value);
        }

        public ShakeSettings Settings { get; private set; }

		/// <summary>
		/// Sets the shake settings.
		/// </summary>
		/// <param name="settings">Settings to use.</param>
		public void SetSettings(ShakeSettings settings)
		{
			Settings = settings;
		}

		/// <summary>
		/// Sets the trauma value, automatically clamped between 0 and 1.
		/// </summary>
		/// <param name="value">The new trauma value.</param>
		public void SetTrauma(float value)
		{
			Trauma = value;
		}

        /// <summary>
        /// Adds a given value to the current trauma, automatically clamped between 0 and 1.
        /// </summary>
        /// <param name="amount">Amount of trauma to add.</param>
        public void AddTrauma(float amount)
		{
			Trauma += amount;
		}

		/// <summary>
		/// Evaluates and returns the shake values, based on a given transform.
		/// </summary>
		/// <param name="transform">Transform to shake.</param>
		/// <returns>The shake values, position as Vector3 and rotation as Quaternion.</returns>
		public (Vector3 pos, Quaternion rot)? Evaluate(Transform transform)
		{
			if (Trauma == 0f)
				return null;

            Vector3 offsetPos = Vector3.zero;

			if ((Settings.PosAxes & CoordinateAxes.X) == CoordinateAxes.X)
				offsetPos += transform.right * (Mathf.PerlinNoise(Time.time * Settings.Speed, 0f) - 0.5f) * 2f;
			if ((Settings.PosAxes & CoordinateAxes.Y) == CoordinateAxes.Y)
				offsetPos += transform.up * (Mathf.PerlinNoise(0f, (Time.time + 5f) * Settings.Speed) - 0.5f) * 2f;
			if ((Settings.PosAxes & CoordinateAxes.Z) == CoordinateAxes.Z)
				offsetPos += transform.forward * (Mathf.PerlinNoise(0f, (Time.time + 10f) * Settings.Speed) - 0.5f) * 2f;

            float sqrTrauma = Trauma * Trauma;
			offsetPos *= Settings.Radius * sqrTrauma;

            Quaternion offsetRot = Quaternion.Euler
			(
				(Settings.RotAxes & CoordinateAxes.X) != CoordinateAxes.X ? 0 : (Mathf.PerlinNoise(Time.time * Settings.Speed, 0f) - 0.5f) * 2f * Settings.XRotMax * sqrTrauma,
				(Settings.RotAxes & CoordinateAxes.Y) != CoordinateAxes.Y ? 0 : (Mathf.PerlinNoise(Time.time * Settings.Speed + 2f, 0f) - 0.5f) * 2f * Settings.YRotMax * sqrTrauma,
				(Settings.RotAxes & CoordinateAxes.Z) != CoordinateAxes.Z ? 0 : (Mathf.PerlinNoise(Time.time * Settings.Speed + 4f, 0f) - 0.5f) * 2f * Settings.ZRotMax * sqrTrauma
			);

			Trauma -= Time.unscaledDeltaTime;
            Trauma = Mathf.Max(Trauma, 0f);

			return (offsetPos, offsetRot);
		}
	}
}