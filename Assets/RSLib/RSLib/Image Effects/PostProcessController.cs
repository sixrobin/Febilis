#if UNITY_POST_PROCESSING_STACK_V2 // Comment this while working on the code.
namespace RSLib.ImageEffects
{
	using RSLib.Maths;
	using UnityEngine;
	using UnityEngine.Rendering.PostProcessing;

	public class PostProcessController : Framework.Singleton<PostProcessController>
	{
		[SerializeField] private PostProcessVolume _postProcessVolume = null;

        public bool TimeScaleDependent = true;
		private System.Collections.Generic.Dictionary<System.Type, System.Collections.IEnumerator> _effectsInterpolationCoroutines
			= new System.Collections.Generic.Dictionary<System.Type, System.Collections.IEnumerator>();


		public bool IsAnyEffectBeingInterpolated()
		{
			foreach (System.Collections.Generic.KeyValuePair<System.Type, System.Collections.IEnumerator> coroutineKvp in _effectsInterpolationCoroutines)
				if (coroutineKvp.Value != null)
					return true;

			return false;
		}

		private bool IsEffectBeingInterpolated<T>() where T : PostProcessEffectSettings
		{
			return _effectsInterpolationCoroutines.TryGetValue(typeof(T), out System.Collections.IEnumerator coroutine) && coroutine != null;
        }

		private bool CanInterpolateEffect<T>() where T : PostProcessEffectSettings
        {
			if (_postProcessVolume == null)
            {
				Debug.LogWarning($"Can not interpolate {typeof(T)} since no target PostProcessVolume is referenced.", gameObject);
				return false;
			}

			if (IsEffectBeingInterpolated<T>())
			{
				Debug.LogWarning($"{typeof(T)} is already being interpolated on volume {_postProcessVolume.transform.name}.", gameObject);
				return false;
			}

			if (!_postProcessVolume.profile.TryGetSettings(out T effect))
			{
				Debug.LogWarning($"Volume {_postProcessVolume.transform.name} doesn't have a {typeof(T)} effect.", gameObject);
				return false;
			}

			if (!effect.active || !effect.enabled)
			{
				Debug.LogWarning($"Volume {_postProcessVolume.transform.name} {typeof(T)} effect is disabled.", gameObject);
				return false;
			}

			return true;
		}

		private void TryStartEffectCoroutine<T>(System.Collections.IEnumerator coroutine) where T : PostProcessEffectSettings
		{
			if (!CanInterpolateEffect<T>())
				return;

			if (!_effectsInterpolationCoroutines.ContainsKey(typeof(T)))
				_effectsInterpolationCoroutines.Add(typeof(T), coroutine);
			else
				_effectsInterpolationCoroutines[typeof(T)] = coroutine;

			StartCoroutine(coroutine);
		}


		#region GENERAL SETTINGS

		public void SetTargetVolume(PostProcessVolume newVolume)
		{
			if (IsAnyEffectBeingInterpolated())
			{
				Debug.LogWarning($"Can not change volume while a fade is occuring on volume {_postProcessVolume.transform.name}.", gameObject);
				return;
			}

			_postProcessVolume = newVolume;
		}

		public void SetIsGlobal(bool state)
		{
			_postProcessVolume.isGlobal = state;
		}

		public void SetWeight(float weight)
		{
			_postProcessVolume.weight = weight;
		}

		public void SetPriority(float priority)
		{
			_postProcessVolume.priority = priority;
		}

		#endregion GENERAL SETTINGS


		#region BLOOM

		public void FadeBloom(float intensity, float threshold, float softKnee, Color color, float duration, Curve curve)
		{
			TryStartEffectCoroutine<Bloom>(FadeBloomCoroutine(intensity, threshold, softKnee, color, duration, curve));
		}

		public void BlinkBloom(float intensity, float threshold, float softKnee, Color color, float inDuration, float waitDuration, float outDuration, Curve inCurve, Curve outCurve)
		{
			TryStartEffectCoroutine<Bloom>(BlinkBloomCoroutine(intensity, threshold, softKnee, color, inDuration, waitDuration, outDuration, inCurve, outCurve));
		}

		public void SetBloom(float intensity, float threshold, float softKnee, Color color, bool forceEnableLayer = false)
		{
			if (!_postProcessVolume.profile.TryGetSettings(out Bloom bloom))
			{
				Debug.LogWarning($"Volume {_postProcessVolume.transform.name} doesn't have a bloom effect.", gameObject);
				return;
			}

			if (forceEnableLayer && !bloom.active || !bloom.enabled)
			{
				bloom.active = true;
				bloom.enabled.Override(true);
			}

			bloom.intensity.Override(intensity);
			bloom.threshold.Override(threshold);
			bloom.softKnee.Override(softKnee);
			bloom.color.Override(color);
		}

		private System.Collections.IEnumerator FadeBloomCoroutine(float intensity, float threshold, float softKnee, Color color, float duration, Curve curve, bool fromBlink = false)
		{
			_postProcessVolume.profile.TryGetSettings(out Bloom bloom);

            float initIntensity = bloom.intensity.value;
			float initThreshold = bloom.threshold.value;
			float initSoftKnee = bloom.softKnee.value;
			Color initColor = bloom.color.value;

			for (float t = 0; t < 1; t += (TimeScaleDependent ? Time.deltaTime : Time.unscaledDeltaTime) / duration)
			{
				bloom.intensity.Override(Mathf.Lerp(initIntensity, intensity, t.Ease(curve)));
				bloom.threshold.Override(Mathf.Lerp(initThreshold, threshold, t.Ease(curve)));
				bloom.softKnee.Override(Mathf.Lerp(initSoftKnee, softKnee, t.Ease(curve)));
				bloom.color.Override(Color.Lerp(initColor, color, t.Ease(curve)));

				yield return null;
			}

			SetBloom(intensity, threshold, softKnee, color);

			if (!fromBlink) // Blink should nullify the coroutine itself.
				_effectsInterpolationCoroutines[typeof(Bloom)] = null;
		}

		private System.Collections.IEnumerator BlinkBloomCoroutine(float intensity, float threshold, float softKnee, Color color, float inDuration, float waitDuration, float outDuration, Curve inCurve, Curve outCurve)
		{
			_postProcessVolume.profile.TryGetSettings(out Bloom bloom);

			float initIntensity = bloom.intensity.value;
			float initThreshold = bloom.threshold.value;
			float initSoftKnee = bloom.softKnee.value;
			Color initColor = bloom.color.value;

			yield return FadeBloomCoroutine(intensity, threshold, softKnee, color, inDuration, inCurve, true);

			if (TimeScaleDependent)
				yield return Yield.SharedYields.WaitForSeconds(waitDuration);
			else
				yield return Yield.SharedYields.WaitForSecondsRealtime(waitDuration);

			yield return FadeBloomCoroutine(initIntensity, initThreshold, initSoftKnee, initColor, outDuration, outCurve, true);

			_effectsInterpolationCoroutines[typeof(Bloom)] = null;
		}

		#endregion BLOOM

		#region CHROMATIC ABERRATION

		public void FadeChromaticAberration(float intensity, float duration, Curve curve)
		{
			TryStartEffectCoroutine<ChromaticAberration>(FadeChromaticAberrationCoroutine(intensity, duration, curve));
		}

		public void BlinkChromaticAberration(float intensity, float inDuration, float waitDuration, float outDuration, Curve inCurve, Curve outCurve)
		{
			TryStartEffectCoroutine<ChromaticAberration>(BlinkChromaticAberrationCoroutine(intensity, inDuration, waitDuration, outDuration, inCurve, outCurve));
		}

		public void SetChromaticAberration(float intensity, bool fastMode, bool forceEnableLayer = false)
		{
			if (!_postProcessVolume.profile.TryGetSettings(out ChromaticAberration chromaticAberration))
			{
				Debug.LogWarning($"Volume {_postProcessVolume.transform.name} doesn't have a chromatic aberration effect.", gameObject);
				return;
			}

			if (forceEnableLayer && !chromaticAberration.active || !chromaticAberration.enabled)
			{
				chromaticAberration.active = true;
				chromaticAberration.enabled.Override(true);
			}

			chromaticAberration.intensity.Override(intensity);
			chromaticAberration.fastMode.Override(fastMode);
		}
		
		private System.Collections.IEnumerator FadeChromaticAberrationCoroutine(float intensity, float duration, Curve curve, bool fromBlink = false)
		{
			_postProcessVolume.profile.TryGetSettings(out ChromaticAberration chromaticAberration);

			float initIntensity = chromaticAberration.intensity.value;

			for (float t = 0; t < 1; t += (TimeScaleDependent ? Time.deltaTime : Time.unscaledDeltaTime) / duration)
			{
				chromaticAberration.intensity.Override(Mathf.Lerp(initIntensity, intensity, t.Ease(curve)));
				yield return null;
			}

			chromaticAberration.intensity.Override(intensity);

			if (!fromBlink) // Blink should nullify the coroutine itself.
				_effectsInterpolationCoroutines[typeof(ChromaticAberration)] = null;
		}

		private System.Collections.IEnumerator BlinkChromaticAberrationCoroutine(float intensity, float inDuration, float waitDuration, float outDuration, Curve inCurve, Curve outCurve)
		{
			_postProcessVolume.profile.TryGetSettings(out ChromaticAberration chromaticAberration);

			float initIntensity = chromaticAberration.intensity.value;

			yield return FadeChromaticAberrationCoroutine(intensity, inDuration, inCurve, true);

			if (TimeScaleDependent)
				yield return Yield.SharedYields.WaitForSeconds(waitDuration);
			else
				yield return Yield.SharedYields.WaitForSecondsRealtime(waitDuration);

			yield return FadeChromaticAberrationCoroutine(initIntensity, outDuration, outCurve, true);

			_effectsInterpolationCoroutines[typeof(ChromaticAberration)] = null;
		}

		#endregion CHROMATIC ABERRATION

		#region DEPTH OF FIELD

		public void FadeDepthOfField(float focusDist, float aperture, float focalLength, float duration, Curve curve)
		{
			TryStartEffectCoroutine<DepthOfField>(FadeDepthOfFieldCoroutine(focusDist, aperture, focalLength, duration, curve));
		}

		public void BlinkDepthOfField(float focusDist, float aperture, float focalLength, float inDuration, float waitDuration, float outDuration, Curve inCurve, Curve outCurve)
		{
			TryStartEffectCoroutine<DepthOfField>(BlinkDepthOfFieldCoroutine(focusDist, aperture, focalLength, inDuration, waitDuration, outDuration, inCurve, outCurve));
		}

		public void SetDepthOfField(float focusDist, float aperture, float focalLength, bool forceEnableLayer = false)
		{
			if (!_postProcessVolume.profile.TryGetSettings(out DepthOfField dof))
			{
				Debug.LogWarning($"Volume {_postProcessVolume.transform.name} doesn't have a depth of field effect.", gameObject);
				return;
			}

			if (forceEnableLayer && !dof.active || !dof.enabled)
			{
				dof.active = true;
				dof.enabled.Override(true);
			}

			dof.focusDistance.Override(focusDist);
			dof.aperture.Override(aperture);
			dof.focalLength.Override(focalLength);
		}

		private System.Collections.IEnumerator FadeDepthOfFieldCoroutine(float focusDist, float aperture, float focalLength, float duration, Curve curve, bool fromBlink = false)
		{
			_postProcessVolume.profile.TryGetSettings(out DepthOfField dof);

            float initFocusDist = dof.focusDistance.value;
			float initAperture = dof.aperture.value;
			float initFocalLength = dof.focalLength.value;

			for (float t = 0; t < 1; t += (TimeScaleDependent ? Time.deltaTime : Time.unscaledDeltaTime) / duration)
			{
				dof.focusDistance.Override(Mathf.Lerp(initFocusDist, focusDist, t.Ease(curve)));
				dof.aperture.Override(Mathf.Lerp(initAperture, aperture, t.Ease(curve)));
				dof.focalLength.Override(Mathf.Lerp(initFocalLength, focalLength, t.Ease(curve)));

				yield return null;
			}

			SetDepthOfField(focusDist, aperture, focalLength);

			if (!fromBlink) // Blink should nullify the coroutine itself.
				_effectsInterpolationCoroutines[typeof(DepthOfField)] = null;
		}

		private System.Collections.IEnumerator BlinkDepthOfFieldCoroutine(float focusDist, float aperture, float focalLength, float inDuration, float waitDuration, float outDuration, Curve inCurve, Curve outCurve)
		{
			_postProcessVolume.profile.TryGetSettings(out DepthOfField dof);

			float initFocusDist = dof.focusDistance.value;
			float initAperture = dof.aperture.value;
			float initFocalLength = dof.focalLength.value;

			yield return FadeDepthOfFieldCoroutine(focusDist, aperture, focalLength, inDuration, inCurve, true);

			if (TimeScaleDependent)
				yield return Yield.SharedYields.WaitForSeconds(waitDuration);
			else
				yield return Yield.SharedYields.WaitForSecondsRealtime(waitDuration);

			yield return FadeDepthOfFieldCoroutine(initFocusDist, initAperture, initFocalLength, outDuration, outCurve, true);

			_effectsInterpolationCoroutines[typeof(DepthOfField)] = null;
		}

		#endregion DEPTH OF FIELD

		#region GRAIN

		public void FadeGrain(float intensity, float size, float luminanceContribution, float duration, Curve curve)
		{
			TryStartEffectCoroutine<Grain>(FadeGrainCoroutine(intensity, size, luminanceContribution, duration, curve));
		}

		public void BlinkGrain(float intensity, float size, float luminanceContribution, float inDuration, float waitDuration, float outDuration, Curve inCurve, Curve outCurve)
		{
			TryStartEffectCoroutine<Grain>(BlinkGrainCoroutine(intensity, size, luminanceContribution, inDuration, waitDuration, outDuration, inCurve, outCurve));
		}

		public void SetGrain(bool colored, float intensity, float size, float luminanceContribution, bool forceEnableLayer = false)
		{
			if (!_postProcessVolume.profile.TryGetSettings(out Grain grain))
			{
				Debug.LogWarning($"Volume {_postProcessVolume.transform.name} doesn't have a grain effect.", gameObject);
				return;
			}

			if (forceEnableLayer && !grain.active || !grain.enabled)
			{
				grain.active = true;
				grain.enabled.Override(true);
			}

			grain.colored.Override(colored);
			grain.intensity.Override(intensity);
			grain.size.Override(size);
			grain.lumContrib.Override(luminanceContribution);
		}

		private System.Collections.IEnumerator FadeGrainCoroutine(float intensity, float size, float luminanceContribution, float duration, Curve curve, bool fromBlink = false)
		{
			_postProcessVolume.profile.TryGetSettings(out Grain grain);

			float initIntensity = grain.intensity.value;
			float initSize = grain.size.value;
			float initLuminanceContribution = grain.lumContrib.value;

			for (float t = 0; t < 1; t += (TimeScaleDependent ? Time.deltaTime : Time.unscaledDeltaTime) / duration)
			{
				grain.intensity.Override(Mathf.Lerp(initIntensity, intensity, t.Ease(curve)));
				grain.size.Override(Mathf.Lerp(initSize, size, t.Ease(curve)));
				grain.lumContrib.Override(Mathf.Lerp(initLuminanceContribution, luminanceContribution, t.Ease(curve)));

				yield return null;
			}

			SetGrain(grain.colored.value, intensity, size, luminanceContribution);

			if (!fromBlink) // Blink should nullify the coroutine itself.
				_effectsInterpolationCoroutines[typeof(Grain)] = null;
		}

		private System.Collections.IEnumerator BlinkGrainCoroutine(float intensity, float size, float luminanceContribution, float inDuration, float waitDuration, float outDuration, Curve inCurve, Curve outCurve)
		{
			_postProcessVolume.profile.TryGetSettings(out Grain grain);

			float initIntensity = grain.intensity.value;
			float initSize = grain.size.value;
			float initLuminanceContribution = grain.lumContrib.value;

			yield return FadeGrainCoroutine(intensity, size, luminanceContribution, inDuration, inCurve, true);

			if (TimeScaleDependent)
				yield return Yield.SharedYields.WaitForSeconds(waitDuration);
			else
				yield return Yield.SharedYields.WaitForSecondsRealtime(waitDuration);

			yield return FadeGrainCoroutine(initIntensity, initSize, initLuminanceContribution, outDuration, outCurve, true);

			_effectsInterpolationCoroutines[typeof(Grain)] = null;
		}

		#endregion GRAIN

		#region LENS DISTORTION

		public void FadeLensDistortion(float intensity, float xMultiplier, float yMultiplier, float centerX, float centerY, float scale, float duration, Curve curve)
		{
			TryStartEffectCoroutine<LensDistortion>(FadeLensDistortionCoroutine(intensity, xMultiplier, yMultiplier, centerX, centerY, scale, duration, curve));
		}

		public void BlinkLensDistortion(float intensity, float xMultiplier, float yMultiplier, float centerX, float centerY, float scale, float inDuration, float waitDuration, float outDuration, Curve inCurve, Curve outCurve)
		{
			TryStartEffectCoroutine<LensDistortion>(BlinkLensDistortionCoroutine(intensity, xMultiplier, yMultiplier, centerX, centerY, scale, inDuration, waitDuration, outDuration, inCurve, outCurve));
		}

		public void SetLensDistortion(float intensity, float xMultiplier, float yMultiplier, float centerX, float centerY, float scale, bool forceEnableLayer = false)
		{
			if (!_postProcessVolume.profile.TryGetSettings(out LensDistortion lensDistortion))
			{
				Debug.LogWarning($"Volume {_postProcessVolume.transform.name} doesn't have a lens distortion effect.", gameObject);
				return;
			}

			if (forceEnableLayer && !lensDistortion.active || !lensDistortion.enabled)
			{
				lensDistortion.active = true;
				lensDistortion.enabled.Override(true);
			}

			lensDistortion.intensity.Override(intensity);
			lensDistortion.intensityX.Override(xMultiplier);
			lensDistortion.intensityY.Override(yMultiplier);
			lensDistortion.centerX.Override(centerX);
			lensDistortion.centerY.Override(centerY);
			lensDistortion.scale.Override(scale);
		}

		private System.Collections.IEnumerator FadeLensDistortionCoroutine(float intensity, float xMultiplier, float yMultiplier, float centerX, float centerY, float scale, float duration, Curve curve, bool fromBlink = false)
		{
			_postProcessVolume.profile.TryGetSettings(out LensDistortion lensDistortion);

			float initIntensity = lensDistortion.intensity.value;
			float initXMultiplier = lensDistortion.intensityX.value;
			float initYMultiplier = lensDistortion.intensityY.value;
			float initCenterX = lensDistortion.centerX.value;
			float initCenterY = lensDistortion.centerY.value;
			float initScale = lensDistortion.scale.value;

			for (float t = 0; t < 1; t += (TimeScaleDependent ? Time.deltaTime : Time.unscaledDeltaTime) / duration)
			{
				lensDistortion.intensity.Override(Mathf.Lerp(initIntensity, intensity, t.Ease(curve)));
				lensDistortion.intensityX.Override(Mathf.Lerp(initXMultiplier, xMultiplier, t.Ease(curve)));
				lensDistortion.intensityY.Override(Mathf.Lerp(initYMultiplier, yMultiplier, t.Ease(curve)));
				lensDistortion.centerX.Override(Mathf.Lerp(initCenterX, centerX, t.Ease(curve)));
				lensDistortion.centerY.Override(Mathf.Lerp(initCenterY, centerY, t.Ease(curve)));
				lensDistortion.scale.Override(Mathf.Lerp(initScale, scale, t.Ease(curve)));

				yield return null;
			}

			SetLensDistortion(intensity, xMultiplier, yMultiplier, centerX, centerY, scale);

			if (!fromBlink) // Blink should nullify the coroutine itself.
				_effectsInterpolationCoroutines[typeof(LensDistortion)] = null;
		}

		private System.Collections.IEnumerator BlinkLensDistortionCoroutine(float intensity, float xMultiplier, float yMultiplier, float centerX, float centerY, float scale, float inDuration, float waitDuration, float outDuration, Curve inCurve, Curve outCurve)
		{
			_postProcessVolume.profile.TryGetSettings(out LensDistortion lensDistortion);

			float initIntensity = lensDistortion.intensity.value;
			float initXMultiplier = lensDistortion.intensityX.value;
			float initYMultiplier = lensDistortion.intensityY.value;
			float initCenterX = lensDistortion.centerX.value;
			float initCenterY = lensDistortion.centerY.value;
			float initScale = lensDistortion.scale.value;

			yield return FadeLensDistortionCoroutine(intensity, xMultiplier, yMultiplier, centerX, centerY, scale, inDuration, inCurve, true);

			if (TimeScaleDependent)
				yield return Yield.SharedYields.WaitForSeconds(waitDuration);
			else
				yield return Yield.SharedYields.WaitForSecondsRealtime(waitDuration);

			yield return FadeLensDistortionCoroutine(initIntensity, initXMultiplier, initYMultiplier, initCenterX, initCenterY, initScale, outDuration, outCurve, true);
		
			_effectsInterpolationCoroutines[typeof(LensDistortion)] = null;
		}

		#endregion LENS DISTORTION

		#region VIGNETTE

		public void FadeVignette(Color color, float intensity, float smoothness, float roundness, float duration, Curve curve)
		{
			TryStartEffectCoroutine<Vignette>(FadeVignetteCoroutine(color, intensity, smoothness, roundness, duration, curve));
		}

		public void BlinkVignette(Color color, float intensity, float smoothness, float roundness, float inDuration, float waitDuration, float outDuration, Curve inCurve, Curve outCurve)
		{
			TryStartEffectCoroutine<Vignette>(BlinkVignetteCoroutine(color, intensity, smoothness, roundness, inDuration, waitDuration, outDuration, inCurve, outCurve));
		}

		public void SetVignette(Color color, float intensity, float smoothness, float roundness, bool rounded, bool forceEnableLayer = false)
		{
			if (!_postProcessVolume.profile.TryGetSettings(out Vignette vignette))
			{
				Debug.LogWarning($"Volume {_postProcessVolume.transform.name} doesn't have a vignette effect.", gameObject);
				return;
			}

			if (forceEnableLayer && !vignette.active || !vignette.enabled)
			{
				vignette.active = true;
				vignette.enabled.Override(true);
			}

			vignette.color.Override(color);
			vignette.intensity.Override(intensity);
			vignette.smoothness.Override(smoothness);
			vignette.roundness.Override(roundness);
			vignette.rounded.Override(rounded);
		}

		private System.Collections.IEnumerator FadeVignetteCoroutine(Color color, float intensity, float smoothness, float roundness, float duration, Curve curve, bool fromBlink = false)
		{
			_postProcessVolume.profile.TryGetSettings(out Vignette vignette);

			Color initColor = vignette.color.value;
			float initIntensity = vignette.intensity.value;
			float initSmoothness = vignette.smoothness.value;
			float initRoundness = vignette.roundness.value;

			for (float t = 0; t < 1; t += (TimeScaleDependent ? Time.deltaTime : Time.unscaledDeltaTime) / duration)
			{
				vignette.color.Override(Color.Lerp(initColor, color, t.Ease(curve)));
				vignette.intensity.Override(Mathf.Lerp(initIntensity, intensity, t.Ease(curve)));
				vignette.smoothness.Override(Mathf.Lerp(initSmoothness, smoothness, t.Ease(curve)));
				vignette.roundness.Override(Mathf.Lerp(initRoundness, roundness, t.Ease(curve)));

				yield return null;
			}

			SetVignette(color, intensity, smoothness, roundness, vignette.rounded.value);

			if (!fromBlink) // Blink should nullify the coroutine itself.
				_effectsInterpolationCoroutines[typeof(Vignette)] = null;
		}

		private System.Collections.IEnumerator BlinkVignetteCoroutine(Color color, float intensity, float smoothness, float roundness, float inDuration, float waitDuration, float outDuration, Curve inCurve, Curve outCurve)
		{
			_postProcessVolume.profile.TryGetSettings(out Vignette vignette);

			Color initColor = vignette.color.value;
			float initIntensity = vignette.intensity.value;
			float initSmoothness = vignette.smoothness.value;
			float initRoundness = vignette.roundness.value;

			yield return FadeVignetteCoroutine(color, intensity, smoothness, roundness, inDuration, inCurve, true);

			if (TimeScaleDependent)
				yield return Yield.SharedYields.WaitForSeconds(waitDuration);
			else
				yield return Yield.SharedYields.WaitForSecondsRealtime(waitDuration);

			yield return FadeVignetteCoroutine(initColor, initIntensity, initSmoothness, initRoundness, outDuration, outCurve, true);

			_effectsInterpolationCoroutines[typeof(Vignette)] = null;
		}

		#endregion VIGNETTE


        private void Reset()
        {
			if (_postProcessVolume == null)
				_postProcessVolume = GetComponent<PostProcessVolume>();
        }
    }
}
#endif