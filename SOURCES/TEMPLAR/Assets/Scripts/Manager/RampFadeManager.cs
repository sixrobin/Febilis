namespace Templar.Manager
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public class RampFadeManager : RSLib.Framework.ConsoleProSingleton<RampFadeManager>
    {
        [SerializeField] private bool _timeScaleDependent = false;
        [SerializeField] private Database.RampFadesDatabase _rampFadesDatabase = null;

        private RSLib.ImageEffects.CameraGrayscaleRamp _ramp;
        private Datas.RampFadeDatas _fadeDatas;
        private FadeEventHandler _callback;

        private static System.Collections.IEnumerator _fadeCoroutine;

        public delegate void FadeEventHandler(bool fadeIn);

        public event FadeEventHandler FadeBegan;
        public event FadeEventHandler FadeOver;

        public static bool IsFading => _fadeCoroutine != null;

        public static bool TimeScaleDependent
        {
            get => Instance._timeScaleDependent;
            set => Instance._timeScaleDependent = value;
        }

        public static void Fade(RSLib.ImageEffects.CameraGrayscaleRamp ramp, Datas.RampFadeDatas fadeDatas, (float, float) delays, FadeEventHandler callback = null)
        {
            UnityEngine.Assertions.Assert.IsTrue(fadeDatas.StepValue > 0f, "Fade step value is set to 0 or less, meaning fade effect would never move on.");

            if (_fadeCoroutine != null)
            {
                Instance.LogWarning("Trying to fade while fade coroutine is already running.");
                return;
            }

            Instance._ramp = ramp;
            Instance._fadeDatas = fadeDatas;
            Instance._callback = callback;

            Instance.StartCoroutine(_fadeCoroutine = Instance.FadeCoroutine(delays));
        }

        public static void Fade(RSLib.ImageEffects.CameraGrayscaleRamp ramp, float targetOffset, float stepDur, float stepValue, (float, float) delays, FadeEventHandler callback = null)
        {
            Datas.RampFadeDatas fadeDatas = Datas.RampFadeDatas.CreateInstance(targetOffset, stepDur, stepValue);
            Fade(ramp, fadeDatas, delays, callback);
        }

        public static void Fade(RSLib.ImageEffects.CameraGrayscaleRamp ramp, string fadeDatasId, (float, float) delays, FadeEventHandler callback = null)
        {
            Datas.RampFadeDatas fadeDatas = Instance._rampFadesDatabase.GetRampFade(fadeDatasId);
            Fade(ramp, fadeDatas, delays, callback);
        }

        public static void SetRampOffset(RSLib.ImageEffects.CameraGrayscaleRamp ramp, float offset)
        {
            ramp.Offset = offset;
        }

        private System.Collections.IEnumerator FadeCoroutine((float, float) delays)
        {
            if (delays.Item1 > 0f)
                yield return RSLib.Yield.SharedYields.WaitForSeconds(delays.Item1);

            _ramp.Offset = _fadeDatas.InitOffset;
            float sign = Mathf.Sign(_fadeDatas.TargetOffset - _ramp.Offset);

            FadeBegan?.Invoke(sign < 0f);

            while (sign == 1f ? _ramp.Offset < _fadeDatas.TargetOffset : _ramp.Offset > _fadeDatas.TargetOffset)
            {
                _ramp.Offset += _fadeDatas.StepValue * sign;
                _ramp.Offset = sign == 1f
                    ? Mathf.Min(_ramp.Offset, _fadeDatas.TargetOffset)
                    : Mathf.Max(_ramp.Offset, _fadeDatas.TargetOffset);

                if (TimeScaleDependent)
                    yield return RSLib.Yield.SharedYields.WaitForSeconds(_fadeDatas.StepDur);
                else
                    yield return RSLib.Yield.SharedYields.WaitForSecondsRealtime(_fadeDatas.StepDur);
            }

            _ramp.Offset = _fadeDatas.TargetOffset;

            if (delays.Item2 > 0f)
                yield return RSLib.Yield.SharedYields.WaitForSeconds(delays.Item2);

            FadeOver?.Invoke(sign < 0f);
            _callback?.Invoke(sign < 0f);

            _fadeCoroutine = null;
        }
    }
}