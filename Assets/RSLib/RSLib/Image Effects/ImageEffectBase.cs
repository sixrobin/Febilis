namespace RSLib.ImageEffects
{
    using UnityEngine;
    
    [AddComponentMenu("")]
    [RequireComponent(typeof (Camera))]
    public class ImageEffectBase : MonoBehaviour
    {
        /// <summary>
        /// Provides a shader property that is set in the inspector
        /// and a material instantiated from the shader.
        /// </summary>
        [SerializeField] private Shader _shader = null;

        private Material _mat;

        protected virtual void Start()
        {
            // Disable the image effect if the shader can't run on the user graphics card.
            if (!_shader || !_shader.isSupported)
                enabled = false;
        }

        protected Material Material
        {
            get
            {
                if (_mat == null)
                    _mat = new Material(_shader) { hideFlags = HideFlags.HideAndDontSave };

                return _mat;
            }
        }

        protected virtual void OnDisable()
        {
            if (_mat)
                DestroyImmediate(_mat);
        }
    }
}