namespace RSLib.ImageEffects
{
    using UnityEngine;

    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("RSLib/Image Effects/Pixelate Effect")]
    public class PixelateEffect : MonoBehaviour
    {
        [SerializeField] private Shader _shader = null;
        [SerializeField] private bool _lockXY = true;
        [SerializeField] private Vector2Int _size = Vector2Int.one;

        private Material _material;
        private int _pixelSizeX;
        private int _pixelSizeY;

        public void SetSizeX(int value)
        {
            _size.x = value;
        }

        public void SetSizeY(int value)
        {
            _size.y = value;
        }

        public void SetSize(int x, int y)
        {
            _size.x = x;
            _size.y = y;
        }

        public void SetSize(Vector2Int size)
        {
            _size = size;
        }

        public void ResetSize()
        {
            _size = Vector2Int.one;
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (_shader == null)
                return;

            if (_material == null)
                _material = new Material(_shader);

            _material.SetInt("_PixelateX", _size.x);
            _material.SetInt("_PixelateY", _size.y);

            Graphics.Blit(source, destination, _material);
        }

        private void OnDisable()
        {
            DestroyImmediate(_material);
        }

        private void Update()
        {
            if (_pixelSizeX != _size.x)
            {
                _pixelSizeX = _size.x;
                if (_lockXY)
                    _pixelSizeY = _size.y = _pixelSizeX;
            }

            if (_pixelSizeY != _size.y)
            {
                _pixelSizeY = _size.y;
                if (_lockXY)
                    _pixelSizeX = _size.x = _pixelSizeY;
            }
        }

        private void OnValidate()
        {
            _size.x = Mathf.Clamp(_size.x, 1, 200);
            _size.y = Mathf.Clamp(_size.y, 1, 200);
        }
    }
}