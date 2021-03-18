﻿namespace Templar
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public class RampFadeManager : RSLib.Framework.Singleton<RampFadeManager>
    {
        [SerializeField] private bool _timeScaleDependent = false;
        [SerializeField] private Datas.RampFadesDatabase _rampFadesDatabase = null;

        private RSLib.ImageEffects.CameraGrayscaleRamp _ramp;
        private Datas.RampFadeDatas _fadeDatas;
        private FadeOverEventHandler _callback;

        private System.Collections.IEnumerator _fadeCoroutine;

        public delegate void FadeOverEventHandler();

        public static bool TimeScaleDependent
        {
            get => Instance._timeScaleDependent;
            set => Instance._timeScaleDependent = value;
        }

        public static void Fade(RSLib.ImageEffects.CameraGrayscaleRamp ramp, Datas.RampFadeDatas fadeDatas, (float, float) delays, FadeOverEventHandler callback = null)
        {
            UnityEngine.Assertions.Assert.IsTrue(fadeDatas.StepValue > 0f, "Fade step value is set to 0 or less, meaning fade effect would never move on.");

            if (Instance._fadeCoroutine != null)
            {
                Instance.LogWarning("Trying to fade while fade coroutine is already running.");
                return;
            }

            Instance._ramp = ramp;
            Instance._fadeDatas = fadeDatas;
            Instance._callback = callback;

            Instance._fadeCoroutine = Instance.FadeCoroutine(delays);
            Instance.StartCoroutine(Instance._fadeCoroutine);
        }

        public static void Fade(RSLib.ImageEffects.CameraGrayscaleRamp ramp, float targetOffset, float stepDur, float stepValue, (float, float) delays, FadeOverEventHandler callback = null)
        {
            Datas.RampFadeDatas fadeDatas = Datas.RampFadeDatas.CreateInstance(targetOffset, stepDur, stepValue);
            Fade(ramp, fadeDatas, delays, callback);
        }

        public static void Fade(RSLib.ImageEffects.CameraGrayscaleRamp ramp, string fadeDatasId, (float, float) delays, FadeOverEventHandler callback = null)
        {
            Datas.RampFadeDatas fadeDatas = Instance._rampFadesDatabase.GetRampFade(fadeDatasId);
            Fade(ramp, fadeDatas, delays, callback);
        }

        private System.Collections.IEnumerator FadeCoroutine((float, float) delays)
        {
            if (delays.Item1 > 0f)
                yield return RSLib.Yield.SharedYields.WaitForSeconds(delays.Item1);

            _ramp.Offset = _fadeDatas.InitOffset;
            float sign = Mathf.Sign(_fadeDatas.TargetOffset - _ramp.Offset);

            while (sign == 1f ? _ramp.Offset < _fadeDatas.TargetOffset : _ramp.Offset > _fadeDatas.TargetOffset)
            {
                _ramp.Offset += _fadeDatas.StepValue * sign;

                if (TimeScaleDependent)
                    yield return RSLib.Yield.SharedYields.WaitForSeconds(_fadeDatas.StepDur);
                else
                    yield return RSLib.Yield.SharedYields.WaitForSecondsRealtime(_fadeDatas.StepDur);
            }

            _ramp.Offset = _fadeDatas.TargetOffset;

            if (delays.Item2 > 0f)
                yield return RSLib.Yield.SharedYields.WaitForSeconds(delays.Item2);

            _callback?.Invoke();
            Instance._fadeCoroutine = null;
        }
    }
}