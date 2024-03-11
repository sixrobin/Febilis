namespace RSLib.Extensions
{
    using UnityEngine;

    public static class AnimationCurveExtensions
    {
        private static readonly Keyframe s_defaultKeyFrame = new Keyframe(0f, 0f);

        #region GENERAL

        /// <summary>
        /// Computes the duration of the curve, using its first and last keyframes on the X axis.
        /// </summary>
        /// <returns>The computed duration of the curve (0 if there is less than 2 keyframes).</returns>
        public static float ComputeDuration(this AnimationCurve curve)
        {
            return curve.GetMaxTime() - curve.GetMinTime();
        }

        /// <summary>
        /// Gets the first keyframe on the X axis of the curve.
        /// </summary>
        /// <returns>The found keyframe, or the default one if there's no keyframe on the curve.</returns>
        public static Keyframe GetFirstKeyframe(this AnimationCurve curve)
        {
            int length = curve.keys.Length;

            if (length == 0)
                return s_defaultKeyFrame;

            int minTimeIndex = 0;
            for (int i = 1; i < length; ++i)
                if (curve.keys[i].time < curve.keys[minTimeIndex].time)
                    minTimeIndex = i;

            return curve.keys[minTimeIndex];
        }

        /// <summary>
        /// Gets the last keyframe on the X axis of the curve.
        /// </summary>
        /// <returns>The found keyframe, or the default one if there's no keyframe on the curve.</returns>
        public static Keyframe GetLastKeyframe(this AnimationCurve curve)
        {
            int length = curve.keys.Length;

            if (length == 0)
                return s_defaultKeyFrame;

            int maxTimeIndex = 0;
            for (int i = 1; i < length; ++i)
                if (curve.keys[i].time > curve.keys[maxTimeIndex].time)
                    maxTimeIndex = i;

            return curve.keys[maxTimeIndex];
        }

        /// <summary>
        /// Gets the minimum time of the curve.
        /// </summary>
        /// <returns>The found minimum time, or default keyframe's if there's no keyframe on the curve.</returns>
        public static float GetMinTime(this AnimationCurve curve)
        {
            return curve.GetFirstKeyframe().time;
        }

        /// <summary>
        /// Gets the maximum time of the curve.
        /// </summary>
        /// <returns>The found maximum time, or default keyframe's if there's no keyframe on the curve.</returns>
        public static float GetMaxTime(this AnimationCurve curve)
        {
            return curve.GetLastKeyframe().time;
        }

        /// <summary>
        /// Gets the lowest keyframe on the Y axis of the curve.
        /// </summary>
        /// <returns>The found keyframe, or the default one if there's no keyframe on the curve.</returns>
        public static Keyframe GetMinValueKeyframe(this AnimationCurve curve)
        {
            int length = curve.keys.Length;

            if (length == 0)
                return s_defaultKeyFrame;

            int minValueIndex = 0;
            for (int i = 1; i < length; ++i)
                if (curve.keys[i].value < curve.keys[minValueIndex].value)
                    minValueIndex = i;

            return curve.keys[minValueIndex];
        }

        /// <summary>
        /// Gets the highest keyframe on the Y axis of the curve.
        /// </summary>
        /// <returns>The found keyframe, or the default one if there's no keyframe on the curve.</returns>
        public static Keyframe GetMaxValueKeyframe(this AnimationCurve curve)
        {
            int length = curve.keys.Length;

            if (length == 0)
                return s_defaultKeyFrame;

            int maxValueIndex = 0;
            for (int i = 1; i < length; ++i)
                if (curve.keys[i].value > curve.keys[maxValueIndex].value)
                    maxValueIndex = i;

            return curve.keys[maxValueIndex];
        }

        /// <summary>
        /// Gets the minimum value of the curve.
        /// </summary>
        /// <returns>The found minimum value, or default keyframe's if there's no keyframe on the curve.</returns>
        public static float GetMinValue(this AnimationCurve curve)
        {
            return curve.GetMinValueKeyframe().value;
        }

        /// <summary>
        /// Gets the maximum value of the curve.
        /// </summary>
        /// <returns>The found maximum value, or default keyframe's if there's no keyframe on the curve.</returns>
        public static float GetMaxValue(this AnimationCurve curve)
        {
            return curve.GetMaxValueKeyframe().value;
        }

        #endregion // GENERAL
    }
}