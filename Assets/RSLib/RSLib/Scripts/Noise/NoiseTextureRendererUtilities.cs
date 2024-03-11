namespace RSLib.Noise
{
    using UnityEngine;

    public class NoiseTextureRendererUtilities : MonoBehaviour
    {
        [SerializeField] private Renderer _renderer = null;

        public void SetRendererTexture(Texture2D texture, bool scaleRenderer = true)
        {
            _renderer.sharedMaterial.mainTexture = texture;
            if (scaleRenderer)
                ScaleRendererWithTexture(texture);
        }

        public void ScaleRendererWithTexture(Texture2D texture)
        {
            _renderer.transform.localScale = new Vector3(texture.width, 1f, texture.height);
        }
    }
}