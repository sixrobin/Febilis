namespace Templar
{
    using UnityEngine;

    public class RampFadeController : RSLib.Framework.Singleton<RampFadeController>
    {
        [SerializeField] private bool _timeScaleDependent = false;

        private RSLib.ImageEffects.CameraGrayscaleRamp _ramp;
        private FadeOverEventHandler _callback;

        private Datas.RampFadeDatas _fadeDatas; // [TODO] Use this.

        // [TODO] Fade datas ScriptableObject ?
        private float _initOffset;
        private float _targetOffset;
        private float _stepDur;
        private float _stepValue;
        private float _delay;

        private System.Collections.IEnumerator _fadeCoroutine;

        public delegate void FadeOverEventHandler();

        public static bool TimeScaleDependent
        {
            get => Instance._timeScaleDependent;
            set => Instance._timeScaleDependent = value;
        }

        public static void Fade()
        {

        }

        public static void Fade(
            RSLib.ImageEffects.CameraGrayscaleRamp ramp,
            float targetOffset,
            float stepDur,
            float stepValue,
            float delay = 0f,
            FadeOverEventHandler callback = null)
        {
            UnityEngine.Assertions.Assert.IsTrue(stepValue > 0f, "Fade step value is set to 0, meaning fade effect would never move on.");

            if (Instance._fadeCoroutine != null)
            {
                Instance.LogWarning("Trying to fade while fade coroutine is already running.");
                return;
            }

            Instance._ramp = ramp;
            Instance._initOffset = Instance._ramp.Offset;
            Instance._targetOffset = targetOffset;
            Instance._stepDur = stepDur;
            Instance._stepValue = stepValue;
            Instance._delay = delay;
            Instance._callback = callback;

            Instance._fadeCoroutine = Instance.FadeCoroutine();
            Instance.StartCoroutine(Instance._fadeCoroutine);
        }

        private System.Collections.IEnumerator FadeCoroutine()
        {
            // Delay should probably not be affected by TimeScaleDependent parameter.
            yield return RSLib.Yield.SharedYields.WaitForSeconds(_delay);

            _ramp.Offset = _initOffset;
            float sign = Mathf.Sign(_targetOffset - _ramp.Offset);

            while (sign == 1f ? _ramp.Offset < _targetOffset : _ramp.Offset > _targetOffset)
            {
                _ramp.Offset += _stepValue * sign;

                if (TimeScaleDependent)
                    yield return RSLib.Yield.SharedYields.WaitForSeconds(_stepDur);
                else
                    yield return RSLib.Yield.SharedYields.WaitForSecondsRealtime(_stepDur);
            }

            _ramp.Offset = _targetOffset;

            _callback?.Invoke();
            Instance._fadeCoroutine = null;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.V))
                Fade(FindObjectOfType<RSLib.ImageEffects.CameraGrayscaleRamp>(), 1f, 0.2f, 0.15f, 0.5f, () => { Debug.Log("over"); });
        }
    }
}