namespace RSLib
{
    using UnityEngine;

    public static class AnimationCurves
    {
        public static AnimationCurve Linear => new AnimationCurve
        (
            new Keyframe(0f, 0f, 0f, 1f),
            new Keyframe(1f, 1f, 1f, 0f)
        );

        public static AnimationCurve LinearReversed => new AnimationCurve
        (
            new Keyframe(0f, 1f, 0f, -1f),
            new Keyframe(1f, 0f, -1f, 0f)
        );

        public static AnimationCurve EaseInOut => new AnimationCurve
        (
            new Keyframe(0f, 0f, 0f, 0f),
            new Keyframe(1f, 1f, 0f, 0f)
        );

        public static AnimationCurve EaseInOutReversed => new AnimationCurve
        (
            new Keyframe(0f, 1f, 0f, 0f),
            new Keyframe(1f, 0f, 0f, 0f)
        );

        public static AnimationCurve One => new AnimationCurve
        (
            new Keyframe(0f, 1f, 0f, 0f),
            new Keyframe(1f, 1f, 0f, 0f)
        );

        public static AnimationCurve Zero => new AnimationCurve
        (
            new Keyframe(0f, 0f, 0f, 0f),
            new Keyframe(1f, 0f, 0f, 0f)
        );
    }
}