namespace RSLib.ImageEffects
{
    using RSLib.Extensions;
    using UnityEngine;

    [ExecuteInEditMode]
    [AddComponentMenu("RSLib/Image Effects/Ramp Grayscale")]
    public class CameraGrayscaleRamp : ImageEffectBase
    {
        /// <summary>
        /// Ramp applied to camera render.
        /// Read/Write must be enabled.
        /// </summary>
        [SerializeField] private Texture2D _textureRamp = null;

        [SerializeField, Range(-1f, 1f)] private float _offset = 0f;
        [SerializeField, Range(0f, 1f)] private float _weight = 1f;

        private Texture2D _initRamp;

        public Texture2D TextureRamp => _textureRamp;

        public bool Inverted { get; set; }

        public float Offset
        {
            get => _offset;
            set => _offset = Mathf.Clamp(value, -1f, 1f);
        }

        public void OverrideRamp(Texture2D ramp)
        {
            _textureRamp = ramp;
        }

        public void ResetInitRamp()
        {
            _textureRamp = _initRamp;
        }

        private void Awake()
        {
            _initRamp = _textureRamp;
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Material.SetTexture("_RampTex", Inverted ? _textureRamp.FlipX() : _textureRamp);
            Material.SetFloat("_RampOffset", _offset);
            Material.SetFloat("_Weight", _weight);

            Graphics.Blit(source, destination, Material);
        }
    }
}