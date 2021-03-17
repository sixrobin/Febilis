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

        [SerializeField, Range(-1f, 1f)] private float _rampOffset = 0f;
        [SerializeField, Range(0f, 1f)] private float _weight = 1f;

        public bool inverted;

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Material.SetTexture("_RampTex", inverted ? _textureRamp.FlipX() : _textureRamp);
            Material.SetFloat("_RampOffset", _rampOffset);
            Material.SetFloat("_Weight", _weight);

            Graphics.Blit(source, destination, Material);
        }
    }
}