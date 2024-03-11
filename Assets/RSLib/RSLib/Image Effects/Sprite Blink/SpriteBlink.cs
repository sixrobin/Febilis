namespace RSLib.ImageEffects
{
	#if RSLIB
	using RSLib.Extensions;
	using RSLib.Maths;
	#endif
	using System.Collections;
	using UnityEngine;
	#if UNITY_EDITOR
	using UnityEditor;
	#endif
	#if ODIN_INSPECTOR
	using Sirenix.OdinInspector;
	#endif

    [DisallowMultipleComponent]
#if UNITY_EDITOR
    [UnityEditor.CanEditMultipleObjects]
#endif
    public class SpriteBlink : MonoBehaviour
	{
        private const string COLOR_SHADER_PARAM = "_Color";
        private const string BLINK_COLOR_SHADER_PARAM = "_BlinkColor";

        #if !ODIN_INSPECTOR
        [Header("GENERAL")]
		#endif
		
		#if ODIN_INSPECTOR
		[FoldoutGroup("General")]
		#endif
		[SerializeField] private Shader _blinkShader = null;
		
		#if ODIN_INSPECTOR
		[FoldoutGroup("General")]
		#endif
		[SerializeField] private bool _useSharedMaterial = false;
		
		#if ODIN_INSPECTOR
		[FoldoutGroup("General")]
		#endif
		[SerializeField] private bool _timeScaleDependent = false;
		
		#if !ODIN_INSPECTOR
        [Header("COLOR BLINK SETTINGS")]
		#endif
		
		#if ODIN_INSPECTOR
		[FoldoutGroup("Color Blink")]
		#endif
		[SerializeField] private Color _color = Color.white;
		
		#if ODIN_INSPECTOR
		[FoldoutGroup("Color Blink")]
		#endif
		[SerializeField, Min(0f)] private float _colorFadeDur = 0.08f;
		
		#if ODIN_INSPECTOR
		[FoldoutGroup("Color Blink")]
		#endif
		[SerializeField, Min(0f)] private float _coloredDur = 0.05f;
		
		#if ODIN_INSPECTOR
		[FoldoutGroup("Color Blink")]
		#endif
		[SerializeField, Min(0f)] private float _inBetweenColorBlinksDur = 0.05f;
		
		#if ODIN_INSPECTOR
		[FoldoutGroup("Color Blink")]
		#endif
        [SerializeField, Range(0f, 1f)] private float _blinkColorAlpha = 1f;
        
        #if RSLIB
		#if ODIN_INSPECTOR
		[FoldoutGroup("Color Blink")]
		#endif
		[SerializeField] private Curve _colorEasingCurve = Curve.InQuad;
		#endif

		#if ODIN_INSPECTOR
		[FoldoutGroup("Color Blink")]
		#endif
		[SerializeField] private bool _colorResetIfBlinking = true;

		#if !ODIN_INSPECTOR
        [Header("ALPHA BLINK SETTINGS")]
		#endif
		
		#if ODIN_INSPECTOR
		[FoldoutGroup("Alpha Blink")]
		#endif
		[SerializeField, Min(0f)] private float _alphaFadeDur = 0.08f;
		
		#if ODIN_INSPECTOR
		[FoldoutGroup("Alpha Blink")]
		#endif
		[SerializeField, Min(0f)] private float _transparencyDur = 0.05f;
		
		#if ODIN_INSPECTOR
		[FoldoutGroup("Alpha Blink")]
		#endif
		[SerializeField, Min(0f)] private float _inBetweenAlphaBlinksDur = 0.05f;
		
		#if ODIN_INSPECTOR
		[FoldoutGroup("Alpha Blink")]
		#endif
		[SerializeField, Range(0f, 1f)] private float _targetAlpha = 0f;
		
		#if RSLIB
		#if ODIN_INSPECTOR
		[FoldoutGroup("Alpha Blink")]
		#endif
		[SerializeField] private Curve _alphaEasingCurve = Curve.InQuad;
		#endif

		#if ODIN_INSPECTOR
		[FoldoutGroup("Alpha Blink")]
		#endif
		[SerializeField] private bool _alphaResetIfBlinking = true;

		private Material _blinkMaterial;
        private IEnumerator _colorCoroutine;
        private IEnumerator _alphaCoroutine;
        
		public bool TimeScaleDependent { get; set; }

		#if ODIN_INSPECTOR
		[Button("Blink (Color)")]
		#endif
		public void BlinkColor(int count = 1, System.Action callback = null)
		{
            UnityEngine.Assertions.Assert.IsTrue(count >= 1, "Can not blink color less than 1 time.");

			if (_colorResetIfBlinking && _colorCoroutine != null)
                StopCoroutine(_colorCoroutine);

			StartCoroutine(_colorCoroutine = BlinkColorCoroutine(count, callback));
		}

		#if ODIN_INSPECTOR
		[Button("Blink (Alpha)")]
		#endif
		public void BlinkAlpha(int count = 1, System.Action callback = null)
		{
            UnityEngine.Assertions.Assert.IsTrue(count >= 1, "Can not blink alpha less than 1 time.");

			if (_alphaResetIfBlinking && _alphaCoroutine != null)
				StopCoroutine(_alphaCoroutine);

			StartCoroutine(_alphaCoroutine = BlinkAlphaCoroutine(count, callback));
		}

		public void ResetColor()
		{
			if (_colorCoroutine != null)
				StopCoroutine(_colorCoroutine);

			#if RSLIB
			_blinkMaterial.SetColor(BLINK_COLOR_SHADER_PARAM, _color.WithA(0f));
			#else
			Color color = new Color(_color.r, _color.g, _color.b, 0f);
			_blinkMaterial.SetColor(BLINK_COLOR_SHADER_PARAM, color);
			#endif
		}

		public void ResetAlpha()
		{
			if (_alphaCoroutine != null)
				StopCoroutine(_alphaCoroutine);

			#if RSLIB
			_blinkMaterial.SetColor(COLOR_SHADER_PARAM, _blinkMaterial.GetColor(COLOR_SHADER_PARAM).WithA(1f));
			#else
			Color color = _blinkMaterial.GetColor(COLOR_SHADER_PARAM);
			color.a = 1f;
			_blinkMaterial.SetColor(BLINK_COLOR_SHADER_PARAM, color);
			#endif
			
		}

        private IEnumerator BlinkColorCoroutine(int count, System.Action callback = null)
		{
            if (count < 1)
                yield break;

            #if !RSLIB
            Color color = _color;
            #endif
            
            for (int i = 0; i < count; ++i)
            {
                for (float t = 0f; t < 1f; t += (TimeScaleDependent ? Time.deltaTime : Time.unscaledDeltaTime) / _colorFadeDur)
                {
	                #if RSLIB
	                _blinkMaterial.SetColor(BLINK_COLOR_SHADER_PARAM, _color.WithA(t.Ease(_colorEasingCurve) * _blinkColorAlpha));
	                #else
	                color.a = t * _blinkColorAlpha;
					_blinkMaterial.SetColor(BLINK_COLOR_SHADER_PARAM, color);
	                #endif
	                
                    yield return null;
                }
                
                #if RSLIB
                _blinkMaterial.SetColor(BLINK_COLOR_SHADER_PARAM, _color.WithA(_blinkColorAlpha));
                #else
	            color.a = _blinkColorAlpha;
				_blinkMaterial.SetColor(BLINK_COLOR_SHADER_PARAM, color);
                #endif

                yield return WaitForDuration(_coloredDur);

                for (float t = 0f; t < 1f; t += (TimeScaleDependent ? Time.deltaTime : Time.unscaledDeltaTime) / _colorFadeDur)
                {
	                #if RSLIB
	                _blinkMaterial.SetColor(BLINK_COLOR_SHADER_PARAM, _color.WithA(_blinkColorAlpha - t.Ease(_colorEasingCurve) * _blinkColorAlpha));
	                #else
	                color.a = _blinkColorAlpha - t * _blinkColorAlpha;
					_blinkMaterial.SetColor(BLINK_COLOR_SHADER_PARAM, color);
	                #endif
	                
                    yield return null;
                }

                #if RSLIB
                _blinkMaterial.SetColor(BLINK_COLOR_SHADER_PARAM, _color.WithA(0f));
                #else
                color.a = 0f;
				_blinkMaterial.SetColor(BLINK_COLOR_SHADER_PARAM, color);
                #endif
                
				if (_inBetweenColorBlinksDur > 0f)
					yield return WaitForDuration(_inBetweenColorBlinksDur);
			}

            callback?.Invoke();
        }

        private IEnumerator BlinkAlphaCoroutine(int count, System.Action callback = null)
		{
            if (count < 1)
                yield break;

            Color initColor = _blinkMaterial.GetColor(COLOR_SHADER_PARAM);

			#if !RSLIB
            Color color = initColor;
            #endif

            for (int i = 0; i < count; ++i)
            {
			    for (float t = 0f; t < 1f; t += (TimeScaleDependent ? Time.deltaTime : Time.unscaledDeltaTime) / _alphaFadeDur)
			    {
				    #if RSLIB
				    _blinkMaterial.SetColor(COLOR_SHADER_PARAM, initColor.WithA(Mathf.Lerp(1f, _targetAlpha, t.Ease(_alphaEasingCurve))));
				    #else
				    color.a = Mathf.Lerp(1f, _targetAlpha, t);
					_blinkMaterial.SetColor(COLOR_SHADER_PARAM, color);
				    #endif
				    
				    yield return null;
			    }

			    #if RSLIB
			    _blinkMaterial.SetColor(COLOR_SHADER_PARAM, initColor.WithA(_targetAlpha));
				#else
	            color.a = _targetAlpha;
				_blinkMaterial.SetColor(COLOR_SHADER_PARAM, color);
				#endif

				yield return WaitForDuration(_transparencyDur);

			    for (float t = 0f; t < 1f; t += (TimeScaleDependent ? Time.deltaTime : Time.unscaledDeltaTime) / _alphaFadeDur)
			    {
				    #if RSLIB
				    _blinkMaterial.SetColor(COLOR_SHADER_PARAM, initColor.WithA(Mathf.Lerp(_targetAlpha, 1f, t.Ease(_alphaEasingCurve))));
				    #else
				    color.a = Mathf.Lerp(_targetAlpha, 1f, t);
					_blinkMaterial.SetColor(COLOR_SHADER_PARAM, color);
				    #endif
				    
				    yield return null;
			    }

			    #if RSLIB
			    _blinkMaterial.SetColor(COLOR_SHADER_PARAM, initColor.WithA(1f));
			    #else
	            color.a = 1f;
				_blinkMaterial.SetColor(COLOR_SHADER_PARAM, color);
			    #endif

				if (_inBetweenAlphaBlinksDur > 0f)
					yield return WaitForDuration(_inBetweenAlphaBlinksDur);
			}

			callback?.Invoke();
        }

        private IEnumerator WaitForDuration(float duration)
        {
	        #if RSLIB
	        if (TimeScaleDependent)
		        yield return Yield.SharedYields.WaitForSeconds(duration);
	        else
		        yield return Yield.SharedYields.WaitForSecondsRealtime(duration);
	        #else
	        if (TimeScaleDependent)
		        yield return new WaitForSeconds(duration);
	        else
		        yield return new WaitForSecondsRealtime(duration);
	        #endif
        }
        
        private void Awake()
		{
			SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
			TimeScaleDependent = _timeScaleDependent;

			_blinkMaterial = _useSharedMaterial ? spriteRenderer.sharedMaterial : spriteRenderer.material;
			_blinkMaterial.shader = _blinkShader;
			
			#if RSLIB
			_blinkMaterial.SetColor(BLINK_COLOR_SHADER_PARAM, _color.WithA(0f));
			#else
			Color color = new Color(_color.r, _color.g, _color.b, 0f);
			_blinkMaterial.SetColor(BLINK_COLOR_SHADER_PARAM, color);
			#endif
        }

		private void OnValidate()
		{
			_colorFadeDur = Mathf.Clamp(_colorFadeDur, 0f, float.MaxValue);
			_alphaFadeDur = Mathf.Clamp(_alphaFadeDur, 0f, float.MaxValue);
			_coloredDur = Mathf.Clamp(_coloredDur, 0f, float.MaxValue);
			_transparencyDur = Mathf.Clamp(_transparencyDur, 0f, float.MaxValue);
		}
	}

	#if RSLIB && UNITY_EDITOR && !ODIN_INSPECTOR
    [CustomEditor(typeof(SpriteBlink))]
    public class SpriteBlinkEditor : EditorUtilities.ButtonProviderEditor<SpriteBlink>
    {
        protected override void DrawButtons()
        {
	        if (!UnityEditor.EditorApplication.isPlaying)
		        return;
	        
	        DrawButton("Blink Color", () => Obj.BlinkColor(1, () => Debug.Log("Color blink over.")));
	        DrawButton("Blink Alpha", () => Obj.BlinkAlpha(1, () => Debug.Log("Alpha blink over.")));
        }
    }
	#endif
}