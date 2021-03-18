namespace RSLib.Maths
{
    using System.Linq;
    
    public static class Maths
    {
        #region AVERAGE

        /// <summary>Computes the average values between bytes.</summary>
        /// <param name="values">Bytes to compute the average between.</param>
        /// <returns>Computed value.</returns>
        public static float ComputeAverageValue(params byte[] values)
        {
            byte sum = 0;
            for (int i = values.Length - 1; i >= 0; --i)
                sum += values[i];

            return (float)sum / values.Length;
        }

        /// <summary>Computes the average values between integers.</summary>
        /// <param name="values">Integers to compute the average between.</param>
        /// <returns>Computed value.</returns>
        public static float ComputeAverageValue(params int[] values)
        {
            int sum = 0;
            for (int i = values.Length - 1; i >= 0; --i)
                sum += values[i];

            return (float)sum / values.Length;
        }

        /// <summary>Computes the average values between floats.</summary>
        /// <param name="values">Floats to compute the average between.</param>
        /// <returns>Computed value.</returns>
        public static float ComputeAverageValue(params float[] values)
        {
            float sum = 0;
            for (int i = values.Length - 1; i >= 0; --i)
                sum += values[i];

            return sum / values.Length;
        }

        /// <summary>Computes the average values between doubles.</summary>
        /// <param name="values">Doubles to compute the average between.</param>
        /// <returns>Computed value.</returns>
        public static double ComputeAverageValue(params double[] values)
        {
            double sum = 0;
            for (int i = values.Length - 1; i >= 0; --i)
                sum += values[i];

            return sum / values.Length;
        }

        /// <summary>
        /// Computes the average position between Vector2s.
        /// This method uses Linq and a foreach loop, using an array would be better if possible.
        /// </summary>
        /// <param name="vectors">Collection of vectors.</param>
        /// <returns>Computed Vector2.</returns>
        public static UnityEngine.Vector2 ComputeAverageVector(System.Collections.Generic.IEnumerable<UnityEngine.Vector2> vectors)
        {
            UnityEngine.Vector2 average = UnityEngine.Vector2.zero;
            foreach (UnityEngine.Vector2 v in vectors)
                average += v;

            average /= vectors.Count();
            return average;
        }

        /// <summary>Computes the average position between Vector2s.</summary>
        /// <param name="vectors">Array of vectors, or multiple vectors as multiple arguments.</param>
        /// <returns>Computed Vector2.</returns>
        public static UnityEngine.Vector2 ComputeAverageVector(params UnityEngine.Vector2[] vectors)
        {
            UnityEngine.Vector2 average = UnityEngine.Vector2.zero;
            for (int i = vectors.Length - 1; i >= 0; --i)
                average += vectors[i];

            average /= vectors.Length;
            return average;
        }

        /// <summary>
        /// Computes the average position between Vector3s.
        /// This method uses Linq and a foreach loop, using an array would be better if possible.
        /// </summary>
        /// <param name="vectors">Collection of vectors.</param>
        /// <returns>Computed Vector3.</returns>
        public static UnityEngine.Vector3 ComputeAverageVector(System.Collections.Generic.IEnumerable<UnityEngine.Vector3> vectors)
        {
            UnityEngine.Vector3 average = UnityEngine.Vector3.zero;
            foreach (UnityEngine.Vector3 v in vectors)
                average += v;

            average /= vectors.Count();
            return average;
        }

        /// <summary>Computes the average position between Vector3s.</summary>
        /// <param name="vectors">Array of vectors, or multiple vectors as multiple arguments.</param>
        /// <returns>Computed Vector3.</returns>
        public static UnityEngine.Vector3 ComputeAverageVector(params UnityEngine.Vector3[] vectors)
        {
            UnityEngine.Vector3 average = UnityEngine.Vector3.zero;
            for (int i = vectors.Length - 1; i >= 0; --i)
                average += vectors[i];

            average /= vectors.Length;
            return average;
        }

        #endregion AVERAGE

        #region CLAMP

        /// <summary>Clamps an integer between two values.</summary>
        /// <param name="i">Value to clamp.</param>
        /// <param name="min">Minimum boundary.</param>
        /// <param name="min">Maximum boundary.</param>
        /// <returns>Clamped value.</returns>
        public static int Clamp(this int i, int min, int max)
        {
            return i < min ? min : i > max ? max : i;
        }

        /// <summary>Clamps a float value between two values.</summary>
        /// <param name="f">Value to clamp.</param>
        /// <param name="min">Minimum boundary.</param>
        /// <param name="min">Maximum boundary.</param>
        /// <returns>Clamped value.</returns>
        public static float Clamp(this float f, float min, float max)
        {
            return f < min ? min : f > max ? max : f;
        }

        /// <summary>Clamps an integer between 0 and 1.</summary>
        /// <param name="i">Value to clamp.</param>
        /// <returns>Clamped value.</returns>
        public static int Clamp01(this int i)
        {
            return i < 0 ? 0 : i > 1 ? 1 : i;
        }

        /// <summary>Clamps a float value between 0 and 1.</summary>
        /// <param name="f">Value to clamp.</param>
        /// <returns>Clamped value.</returns>
        public static float Clamp01(this float f)
        {
            return f < 0 ? 0 : f > 1 ? 1 : f;
        }

        #endregion CLAMP

        #region GENERAL

        /// <summary>Eases a value between 0 and 1.
        /// https://www.youtube.com/watch?v=3D0PeJh6GY8 </summary>
        /// <param name="x">Value to ease clamped between 0 and 1.</param>
        /// <param name="easeAmount">Easing amount clamped above 0.</param>
        /// <returns>Eased value.</returns>
        public static float ComputeBasicEasing(this float x, float easeAmount)
        {
            float xClamped = x.Clamp01();
            float easeAmountClamped = easeAmount.Clamp(0f, float.MaxValue);

            float a = Clamp(easeAmountClamped + 1f, 0f, float.MaxValue);
            float xPowA = (float)System.Math.Pow(xClamped, a);
            return xPowA / (xPowA + (float)System.Math.Pow(1f - xClamped, a));
        }

        /// <summary>Computes the fractional part (decimal part) of a float.</summary>
        /// <param name="f">Value to get fractional part of.</param>
        /// <returns>Fractional part.</returns>
        public static float ComputeFractionalPart(this float f)
        {
            return f - (float)System.Math.Floor(f);
        }

        /// <summary>Computes the fractional part (decimal part) of a double.</summary>
        /// <param name="d">Value to get fractional part of.</param>
        /// <returns>Fractional part.</returns>
        public static double ComputeFractionalPart(this double d)
        {
            return d - System.Math.Floor(d);
        }

        /// <summary>Computes the greatest common divisor of two integers.</summary>
        /// <param name="a">First integer.</param>
        /// <param name="b">Second integer.</param>
        /// <returns>Greatest common divisor.</returns>
        public static int ComputeGreatestCommonDivisor(int a, int b)
        {
            while (b != 0)
            {
                int c = a % b;
                a = b;
                b = c;
            }

            return System.Math.Abs(a);
        }

        /// <summary>Computes factorial of x (x!).</summary>
        /// <param name="i">Value to compute factorial of.</param>
        /// <returns>Factorial of x.</returns>
        public static int Factorial(this int i)
        {
            int factorial = 1;
            for (int p = i; p >= 1; --p)
                factorial *= p;

            return factorial;
        }

        /// <summary>Inverses the value (1 / x).</summary>
        /// <param name="f">Value to inverse.</param>
        /// <returns>Inversed value.</returns>
        public static float Inverse(this float f)
        {
            return 1f / f;
        }

        /// <summary>Inverses the value (1 / x).</summary>
        /// <param name="d">Value to inverse.</param>
        /// <returns>Inversed value.</returns>
        public static double Inverse(this double d)
        {
            return 1 / d;
        }

        /// <summary>Checks if an integer value is even.</summary>
        /// <param name="i">Integer to evaluate.</param>
        /// <returns>True if it's even, false if it's odd.</returns>
        public static bool IsEven(this int i)
        {
            return i % 2 == 0;
        }

        /// <summary>Computes the result of 1 - x.</summary>
        /// <param name="f">Value to switch.</param>
        /// <returns>Switched value.</returns>
        public static float OneMinus(this float f)
        {
            return 1f - f;
        }

        /// <summary>Computes the result of 1 - x.</summary>
        /// <param name="d">Value to switch.</param>
        /// <returns>Switched value.</returns>
        public static double OneMinus(this double d)
        {
            return 1 - d;
        }

        #endregion GENERAL

        #region NORMALIZATION

        /// <summary>Brings any value in a given range to an unclamped custom range.
        /// <param name="x">Value to normalize.</param>
        /// <param name="r1Min">Minimum range.</param>
        /// <param name="r1Max">Maximum range.</param>
        /// <param name="r2Min">Target range minimum value.</param>
        /// <param name="r2Max">Target range maximum value.</param>
        /// <returns>Normalized value.</returns>
        public static float Normalize(this float x, float r1Min, float r1Max, float r2Min, float r2Max)
        {
            return r2Min + (x - r1Min) * (r2Max - r2Min) / (r1Max - r1Min);
        }

        /// <summary>Brings any value in a given range to a clamped custom range.
        /// <param name="x">Value to normalize.</param>
        /// <param name="r1Min">Minimum range.</param>
        /// <param name="r1Max">Maximum range.</param>
        /// <param name="r2Min">Target range minimum value.</param>
        /// <param name="r2Max">Target range maximum value.</param>
        /// <returns>Normalized value.</returns>
        public static float NormalizeClamped(this float x, float r1Min, float r1Max, float r2Min, float r2Max)
        {
            return Normalize(x, r1Min, r1Max, r2Min, r2Max).Clamp(r2Min, r2Max);
        }

        /// <summary>Brings any value in a given range to the [0,1] unclamped range.</summary>
        /// <param name="x">Value to normalize.</param>
        /// <param name="rMin">Minimum range.</param>
        /// <param name="rMax">Maximum range.</param>
        /// <returns>Normalized value.</returns>
        public static float Normalize01(this float x, float rMin, float rMax)
        {
            return (x - rMin) / (rMax - rMin);
        }

        /// <summary>Brings any value in a given range to the [0,1] clamped range.</summary>
        /// <param name="x">Value to normalize.</param>
        /// <param name="rMin">Minimum range.</param>
        /// <param name="rMax">Maximum range.</param>
        /// <returns>Normalized value.</returns>
        public static float Normalize01Clamped(this float x, float rMin, float rMax)
        {
            return Normalize01(x, rMin, rMax).Clamp01();
        }

        /// <summary>Computes the normalized value for a given angle.</summary>
        /// <returns>Angle value between 0 and 360.</returns>
        public static float NormalizeAngle(this float a)
        {
            a %= 360f;
            if (a < 0)
                a += 360f;

            return a;
        }

        #endregion NORMALIZATION

        #region PERCENTAGE

        /// <summary>Computes the base 1 percentage of two values.</summary>
        /// <param name="current">Current value.</param>
        /// <param name="total">Total.</param>
        /// <returns>Percentage between 0 and 1.</returns>
        public static float ComputeBase1Percentage(this float current, float total)
        {
            return current / total;
        }

        /// <summary> Computes the base 100 percentage of two values.</summary>
        /// <param name="current">Current value.</param>
        /// <param name="total">Total.</param>
        /// <returns>Percentage between 0 and 100.</returns>
        public static float ComputeBase100Percentage(this float current, float total)
        {
            return ComputeBase1Percentage(current, total) * 100f;
        }

        /// <summary>Computes the base 100 percentage of two values and rounds it to the closest integer.</summary>
        /// <param name="current">Current value.</param>
        /// <param name="total">Total</param>
        /// <returns>Rounded percentage between 0 and 100.</returns>
        public static int ComputeBase100PercentageRounded(this float current, float total)
        {
            return (int)System.Math.Round(ComputeBase100Percentage(current, total));
        }

        /// <summary>Computes the unclamped percentage of a value.</summary>
        /// <param name="percentage">Percentage to compute between 0 and 100.</param>
        /// <param name="total">Total.</param>
        /// <returns>Percentage value.</returns>
        public static float ComputePercentage(float percentage, float total)
        {
            return percentage * 0.01f * total;
        }

        /// <summary>Computes the unclamped rounded percentage of a value.</summary>
        /// <param name="percentage">Percentage to compute between 0 and 100.</param>
        /// <param name="total">Total.</param>
        /// <returns>Rounded percentage value.</returns>
        public static int ComputePercentageRounded(float percentage, float total)
        {
            return (int)System.Math.Round(percentage * 0.01f * total);
        }

        #endregion PERCENTAGE

        #region POWER

        /// <summary>Cubes a given integer.</summary>
        /// <returns>Cubed integer.</returns>
        public static int Cubed(this int i)
        {
            return i * i * i;
        }

        /// <summary>Cubes a given float.</summary>
        /// <returns>Cubed float.</returns>
        public static float Cubed(this float f)
        {
            return f * f * f;
        }

        /// <summary>Cubes a given double.</summary>
        /// <returns>Cubed double.</returns>
        public static double Cubed(this double d)
        {
            return d * d * d;
        }

        /// <summary>Squares a given integer.</summary>
        /// <returns>Squared integer.</returns>
        public static int Sqr(this int i)
        {
            return i * i;
        }

        /// <summary>Squares a given float.</summary>
        /// <returns>Squared float.</returns>
        public static float Sqr(this float f)
        {
            return f * f;
        }

        /// <summary>Squares a given double.</summary>
        /// <returns>Squared double.</returns>
        public static double Sqr(this double d)
        {
            return d * d;
        }

        #endregion POWER
    }
}