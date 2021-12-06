namespace Templar.Datas
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Ramp Fade Datas", menuName = "Datas/Ramps/Ramp Fade")]
    public class RampFadeDatas : ScriptableObject
    {
        [Tooltip("Ramp offset at the end of the fade.")]
        [SerializeField, Range(-1f, 1f)] private float _targetOffset = 0f;

        [Tooltip("Duration between two steps of the fade coroutine.")]
        [SerializeField, Min(0f)] private float _stepDur = 0.2f;

        [Tooltip("Value applied to ramp offset at each step of the fade coroutine.")]
        [SerializeField, Min(0.001f)] private float _stepValue = 0.2f;

        [Tooltip("If unchecked, fade will start from the ramp current value, else, ramp value will be overridden.")]
        [SerializeField] private bool _overrideInitOffset = false;

        [Tooltip("Value to apply to ramp offset on fade start, if override option is checked.")]
        [SerializeField, Range(-1f, 1f)] private float _initOffset = 0f;

        public float TargetOffset => _targetOffset;
        public float StepDur => _stepDur;
        public float StepValue => _stepValue;

        public bool OverrideInitOffset => _overrideInitOffset;
        public float InitOffset => _initOffset;

        public static RampFadeDatas CreateInstance(float targetOffset, float stepDur, float stepValue)
        {
            RampFadeDatas fadeDatas = CreateInstance<RampFadeDatas>();
            fadeDatas._targetOffset = targetOffset;
            fadeDatas._stepDur = stepDur;
            fadeDatas._stepValue = stepValue;
            fadeDatas._overrideInitOffset = false;

            return fadeDatas;
        }

        public static RampFadeDatas CreateInstanceWithInitOffset(float initOffset, float targetOffset, float stepDur, float stepValue)
        {
            RampFadeDatas fadeDatas = CreateInstance<RampFadeDatas>();
            fadeDatas._targetOffset = targetOffset;
            fadeDatas._stepDur = stepDur;
            fadeDatas._stepValue = stepValue;

            fadeDatas._overrideInitOffset = true;
            fadeDatas._initOffset = initOffset;

            return fadeDatas;
        }
    }
}