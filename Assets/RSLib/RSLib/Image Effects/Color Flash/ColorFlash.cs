namespace RSLib.ImageEffects
{
    using RSLib.Extensions;
    using RSLib.Maths;
    using UnityEngine;

    public class ColorFlash : RSLib.Framework.Singleton<ColorFlash>
    {
        [System.Serializable]
        public struct ColorFlashData
        {
            public Color Color;
            public float? Alpha;
            public float InDuration;
            public float Duration;
            public float OutDuration;
            public Curve InCurve;
            public Curve OutCurve;

            public float TotalDuration => InDuration + Duration + OutDuration;
        }
        
        [SerializeField] private UnityEngine.UI.Image _flashImage = null;

        public bool IsFlashing => _flashImage.enabled;

        public static void Flash(ColorFlashScriptable scriptableData, System.Action inCallback = null, System.Action outCallback = null)
        {
            ColorFlashData data = new ColorFlashData
            {
                Color = scriptableData.Color,
                Alpha = scriptableData.Alpha,
                InDuration = scriptableData.InDuration,
                Duration = scriptableData.Duration,
                OutDuration = scriptableData.OutDuration,
                InCurve = scriptableData.InCurve,
                OutCurve = scriptableData.OutCurve
            };
            
            Flash(data, inCallback, outCallback);
        }
        
        public static void Flash(ColorFlashData data, System.Action inCallback = null, System.Action outCallback = null)
        {
            if (!Exists())
            {
                LogWarningStatic("Trying to play a color flash but no instance exists!");
                return;
            }

            if (data.TotalDuration <= 0f)
            {
                Instance.LogWarning($"Trying to play a color flash with a negative total duration ({data.TotalDuration})!", Instance.gameObject);
                return;
            }
            
            if (Instance._flashImage == null)
            {
                Instance.LogWarning($"Missing image reference on {nameof(ColorFlash)} instance!", Instance.gameObject);
                return;
            }

            if (Instance.IsFlashing)
            {
                Debug.Log($"Trying to play a color flash with {nameof(ColorFlash)} instance is already flashing!", Instance.gameObject);
                return;
            }
            
            Instance.StartCoroutine(FlashCoroutine(data, inCallback, outCallback));
        }

        private static System.Collections.IEnumerator FlashCoroutine(ColorFlashData data, System.Action inCallback, System.Action outCallback)
        {
            Instance._flashImage.enabled = true;
            Color color = data.Color.WithA(data.Alpha ?? data.Color.a);

            if (data.InDuration > 0f)
            {
                for (float t = 0f; t < 1f; t += Time.deltaTime / data.InDuration)
                {
                    Instance._flashImage.color = color.WithA(t.Ease(data.InCurve));
                    yield return null;
                }
            }
            
            Instance._flashImage.color = color;
            inCallback?.Invoke();

            if (data.Duration > 0f)
                yield return RSLib.Yield.SharedYields.WaitForSeconds(data.Duration);

            if (data.OutDuration > 0f)
            {
                for (float t = 0f; t < 1f; t += Time.deltaTime / data.OutDuration)
                {
                    Instance._flashImage.color = color.WithA(1f - t.Ease(data.OutCurve));
                    yield return null;
                }
            }
            
            Instance._flashImage.enabled = false;
            outCallback?.Invoke();
        }

        protected override void Awake()
        {
            base.Awake();
            if (!IsValid)
                return;

            if (Instance._flashImage == null)
            {
                Debug.LogWarning($"Missing image reference on {nameof(ColorFlash)} instance!", Instance.gameObject);
                return;
            }
            
            _flashImage.enabled = false;
        }
    }
}