namespace Templar.Datas
{
    using UnityEngine;

    public class RampFadeDatas : ScriptableObject
    {
        [Tooltip("Ramp offset at the end of the fade.")]
        [SerializeField, Range(-1f, 1f)] private float _targetOffset = 0f;

        [Tooltip("Duration between two steps of the fade coroutine.")]
        [SerializeField, Min(0f)] private float _stepDur = 0.2f;

        [Tooltip("Value applied to ramp offset at each step of the fade coroutine.")]
        [SerializeField, Min(0.01f)] private float _stepValue = 0.2f;

        [Tooltip("Delay to wait before starting the fade.")]
        [SerializeField, Min(0f)] private float _delay = 0f;

        [Tooltip("If unchecked, fade will start from the ramp current value, else, ramp value will be overridden.")]
        [SerializeField] private bool _overrideInitOffset = false;

        [Tooltip("Value to apply to ramp offset on fade start, if override option is checked.")]
        [SerializeField, Range(-1f, 1f)] private float _initOffset = 0f;

        public float TargetOffset => _targetOffset;
        public float StepDur => _stepDur;
        public float StepValue => _stepValue;
        public float Delay => _delay;
        public bool HasDelay => Delay > 0f;

        public bool OverrideInitOffset => _overrideInitOffset;
        public float InitOffset => _initOffset;
    }
}