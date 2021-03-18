namespace RSLib.ImageEffects
{
    using UnityEngine;

    /// <summary>
    /// Used to emit ripple effect on screen.
    /// Can emit up to 5 droplets at the same time (not procedural due to shader code).
    /// </summary>
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("RSLib/Image Effects/Ripple Effect")]
    public class RippleEffect : RSLib.Framework.Singleton<RippleEffect>
    {
        private class Droplet
        {
            private Vector2 _pos;
            private float _time;
            private bool _timeScaleDependent;

            public Droplet()
            {
                _time = 1000f;
            }

            public Droplet(bool timeScaleDependent)
            {
                _time = 1000f;
                _timeScaleDependent = timeScaleDependent;
            }

            public void Reset(float screenX, float screenY)
            {
                _pos = new Vector2(screenX, screenY);
                _time = 0f;
            }

            public void Update()
            {
                _time += _timeScaleDependent ? Time.deltaTime : Time.unscaledDeltaTime;
            }

            public Vector4 MakeShaderParameter(float aspect)
            {
                return new Vector4(_pos.x * aspect, _pos.y, _time, 0f);
            }
        }

        [Header("SHADER")]
        [SerializeField] private Shader _shader = null;

        [Header("DROPLET WAVE")]
        [SerializeField]
        private AnimationCurve _waveform = new AnimationCurve
        (
            new Keyframe(0.00f, 0.50f, 0f, 0f),
            new Keyframe(0.05f, 1.00f, 0f, 0f),
            new Keyframe(0.15f, 0.10f, 0f, 0f),
            new Keyframe(0.25f, 0.80f, 0f, 0f),
            new Keyframe(0.35f, 0.30f, 0f, 0f),
            new Keyframe(0.45f, 0.60f, 0f, 0f),
            new Keyframe(0.55f, 0.40f, 0f, 0f),
            new Keyframe(0.65f, 0.55f, 0f, 0f),
            new Keyframe(0.75f, 0.46f, 0f, 0f),
            new Keyframe(0.85f, 0.52f, 0f, 0f),
            new Keyframe(0.99f, 0.50f, 0f, 0f)
        );

        [Header("DROPLET SETTINGS")]
        [SerializeField, Range(0.01f, 1f)] private float _refractionStrength = 0.5f;
        [SerializeField, Range(0.01f, 1f)] private float _reflectionStrength = 0.7f;
        [SerializeField, Range(1f, 3f)] private float _waveSpeed = 1.25f;
        [SerializeField] private Color _reflectionColor = Color.gray;
        [SerializeField] private bool _timeScaleDependent = false;

        private Camera _cam;
        private Droplet[] _droplets;
        private Texture2D _gradTexture;
        private Material _material;

        int _nextDropletIndex;
        public int NextDropletIndex
        {
            get => _nextDropletIndex;
            set => _nextDropletIndex = ++_nextDropletIndex % _droplets.Length;
        }

        public static void RippleAtWorldPosition(Vector3 pos)
        {
            Vector3 screenPos = Instance._cam.WorldToViewportPoint(pos);
            Instance.Emit(screenPos.x, screenPos.y);
        }

        public static void RippleAtWorldPosition(float x, float y)
        {
            Vector3 viewportPos = Instance._cam.WorldToViewportPoint(new Vector3(x, y));
            Instance.Emit(viewportPos.x, viewportPos.y);
        }

        private void Initialize()
        {
            _cam = GetComponent<Camera>();

            _droplets = new Droplet[5]
            {
                new Droplet(_timeScaleDependent),
                new Droplet(_timeScaleDependent),
                new Droplet(_timeScaleDependent),
                new Droplet(_timeScaleDependent),
                new Droplet(_timeScaleDependent)
            };

            _gradTexture = new Texture2D(2048, 1, TextureFormat.Alpha8, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };

            for (int i = 0; i < _gradTexture.width; ++i)
            {
                float waveEval = _waveform.Evaluate(1f / _gradTexture.width * i);
                _gradTexture.SetPixel(i, 0, new Color(waveEval, waveEval, waveEval, waveEval));
            }

            _gradTexture.Apply();

            _material = new Material(_shader) { hideFlags = HideFlags.DontSave };
            _material.SetTexture("_GradTex", _gradTexture);

            UpdateShaderParameters();
        }

        private void UpdateShaderParameters()
        {
            for (int i = 0; i < _droplets.Length; ++i)
                _material.SetVector($"_Drop{i + 1}", _droplets[i].MakeShaderParameter(_cam.aspect));

            _material.SetColor("_Reflection", _reflectionColor);
            _material.SetVector("_Params1", new Vector4(_cam.aspect, 1f, 1f / _waveSpeed, 0f));
            _material.SetVector("_Params2", new Vector4(1f, 1f / _cam.aspect, _refractionStrength, _reflectionStrength));
        }

        private void Emit(float x, float y)
        {
            _droplets[NextDropletIndex++].Reset(x, y);
        }

        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }

        private void Update()
        {
            for (int i = _droplets.Length - 1; i >= 0; --i)
                _droplets[i].Update();

            UpdateShaderParameters();
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(source, destination, _material);
        }
    }
}