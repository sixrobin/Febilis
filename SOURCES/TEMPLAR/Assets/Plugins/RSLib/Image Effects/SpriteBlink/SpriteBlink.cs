namespace RSLib.ImageEffects
{
	using RSLib.Extensions;
	using RSLib.Maths;
	using System.Collections;
	using UnityEngine;

    [DisallowMultipleComponent]
#if UNITY_EDITOR
    [UnityEditor.CanEditMultipleObjects]
#endif
    public class SpriteBlink : MonoBehaviour
	{
		private Material _blinkMat;

		private IEnumerator _colorCoroutine;
        private IEnumerator _alphaCoroutine;

		[SerializeField] private Shader _blinkShader = null;

		[Header("COLOR BLINK SETTINGS")]
		[SerializeField] private Color _color = Color.white;
		[SerializeField] private float _colorFadeDur = 0.08f;
		[SerializeField] private float _coloredDur = 0.05f;
        [SerializeField, Range(0f, 1f)] private float _blinkColorAlpha = 1f;
		[SerializeField] private Curve _colorEasingCurve = Curve.InQuad;
		[SerializeField] private bool _colorResetIfBlinking = true;

		[Header("ALPHA BLINK SETTINGS")]
		[SerializeField] private float _alphaFadeDur = 0.08f;
		[SerializeField] private float _transparencyDur = 0.05f;
		[SerializeField, Range(0f, 1f)] private float _targetAlpha = 0f;
		[SerializeField] private Curve _alphaEasingCurve = Curve.InQuad;
		[SerializeField] private bool _alphaResetIfBlinking = true;

        [Header("GENERAL")]
        private bool _timeScaleDependent = false;

		public bool TimeScaleDependent { get; set; }

		IEnumerator BlinkColorCoroutine(int count)
		{
            if (count < 1)
                yield break;

            for (int i = 0; i < count; ++i)
            {
                for (float t = 0f; t < 1f; t += (TimeScaleDependent ? Time.deltaTime : Time.unscaledDeltaTime) / _colorFadeDur)
                {
                    _blinkMat.SetColor("_BlinkColor", _color.WithA(Easing.Ease(t, _colorEasingCurve) * _blinkColorAlpha));
                    yield return null;
                }

                _blinkMat.SetColor("_BlinkColor", _color.WithA(_blinkColorAlpha));

                if (TimeScaleDependent)
                    yield return Yield.SharedYields.WaitForSeconds(_coloredDur);
                else
                    yield return Yield.SharedYields.WaitForSecondsRealtime(_coloredDur);

                for (float t = 0f; t < 1f; t += (TimeScaleDependent ? Time.deltaTime : Time.unscaledDeltaTime) / _colorFadeDur)
                {
                    _blinkMat.SetColor("_BlinkColor", _color.WithA(_blinkColorAlpha - Easing.Ease(t, _colorEasingCurve) * _blinkColorAlpha));
                    yield return null;
                }

                _blinkMat.SetColor("_BlinkColor", _color.WithA(0f));
            }
		}

		IEnumerator BlinkAlphaCoroutine(int count)
		{
            if (count < 1)
                yield break;

            for (int i = 0; i < count; ++i)
            {
			    Color initColor = _blinkMat.GetColor("_Color");

			    for (float t = 0f; t < 1f; t += (TimeScaleDependent ? Time.deltaTime : Time.unscaledDeltaTime) / _alphaFadeDur)
			    {
				    _blinkMat.SetColor("_Color", initColor.WithA(Mathf.Lerp (1f, _targetAlpha, Easing.Ease(t, _alphaEasingCurve))));
				    yield return null;
			    }

			    _blinkMat.SetColor("_Color", initColor.WithA(_targetAlpha));

			    if (TimeScaleDependent)
                    yield return Yield.SharedYields.WaitForSeconds(_transparencyDur);
			    else
                    yield return Yield.SharedYields.WaitForSecondsRealtime(_transparencyDur);

			    for (float t = 0f; t < 1f; t += (TimeScaleDependent ? Time.deltaTime : Time.unscaledDeltaTime) / _alphaFadeDur)
			    {
				    _blinkMat.SetColor("_Color", initColor.WithA(Mathf.Lerp(_targetAlpha, 1f, Easing.Ease(t, _alphaEasingCurve))));
				    yield return null;
			    }

			    _blinkMat.SetColor("_Color", initColor.WithA(1f));
            }
        }

		public void BlinkColor(int count = 1)
		{
            UnityEngine.Assertions.Assert.IsTrue(count >= 1, "Can not blink color less than 1 time.");

			if (_colorResetIfBlinking && _colorCoroutine != null)
                StopCoroutine(_colorCoroutine);

            _colorCoroutine = BlinkColorCoroutine(count);
			StartCoroutine(_colorCoroutine);
		}

		public void BlinkAlpha(int count = 1)
		{
            UnityEngine.Assertions.Assert.IsTrue(count >= 1, "Can not blink alpha less than 1 time.");

			if (_alphaResetIfBlinking && _alphaCoroutine != null)
				StopCoroutine(_alphaCoroutine);

			_alphaCoroutine = BlinkAlphaCoroutine(count);
			StartCoroutine(_alphaCoroutine);
		}

		public void ResetColor()
		{
			StopCoroutine(_colorCoroutine);
			_blinkMat.SetColor("_BlinkColor", _color.WithA(0f));
		}

		public void ResetAlpha()
		{
			StopCoroutine(_alphaCoroutine);
			_blinkMat.SetColor("_Color", _blinkMat.GetColor("_Color").WithA(1f));
		}

        private void Awake()
		{
			_blinkMat = GetComponent<SpriteRenderer>().material;
			_blinkMat.shader = _blinkShader;
			_blinkMat.SetColor("_BlinkColor", _color.WithA(0));
            TimeScaleDependent = _timeScaleDependent;
        }

		private void OnValidate()
		{
			_colorFadeDur = Mathf.Clamp(_colorFadeDur, 0f, float.MaxValue);
			_alphaFadeDur = Mathf.Clamp(_alphaFadeDur, 0f, float.MaxValue);
			_coloredDur = Mathf.Clamp(_coloredDur, 0f, float.MaxValue);
			_transparencyDur = Mathf.Clamp(_transparencyDur, 0f, float.MaxValue);
		}
	}
}