namespace RSLib.Noise
{
    using UnityEngine;

    public class NoiseMapGenerator : MonoBehaviour
    {
        [System.Serializable]
        public class ColorByHeight
        {
            [SerializeField] private float _height = 0f;
            [SerializeField] private Color _color = Color.white;
            
            public float Height => _height;
            public Color Color => _color;
        }

        [Header("REFS")]
        [SerializeField] private NoiseTextureRendererUtilities _noiseMapView = null;

        [Header("NOISE DATA")]
        [SerializeField] private Vector2Int _size = Vector2Int.one;
        [SerializeField, Min(0.0001f)] private float _noiseScale = 1f;
        [SerializeField] private int _seed = 0;
        [SerializeField, Min(0f)] private int _octaves = 4;
        [SerializeField, Range(0f, 1f)] private float _persistance = 0.5f;
        [SerializeField, Min(1f)] private float _lacunarity = 2f;
        [SerializeField] private Vector2 _offset = Vector2.zero;
        [SerializeField] private FilterMode _filterMode = FilterMode.Bilinear;

        [Header("COLORS")]
        [SerializeField] private ColorMode _colorMode = ColorMode.NOISE_MAP;
        [SerializeField] private Color _heightMapColorA = Color.white;
        [SerializeField] private Color _heightMapColorB = Color.black;
        [SerializeField] private Maths.Curve _heightMapLerpCurve = Maths.Curve.Linear;
        [SerializeField] private ColorByHeight[] _colors = null;

        public enum ColorMode
        {
            [InspectorName("Noise Map")] NOISE_MAP,
            [InspectorName("Color Map")] COLOR_MAP
        }

        public void GenerateMap()
        {
            float[,] noiseMap = Noise.GenerateNoiseMap(_size, _noiseScale, _seed, _octaves, _persistance, _lacunarity, _offset);

            Color[] colorMap = new Color[_size.x * _size.y];
            for (int x = 0; x < _size.x; ++x)
            {
                for (int y = 0; y < _size.y; ++y)
                {
                    float currHeight = noiseMap[x, y];
                    for (int i = 0; i < _colors.Length; ++i)
                    {
                        if (currHeight <= _colors[i].Height)
                        {
                            colorMap[x + _size.y * y] = _colors[i].Color;
                            break;
                        }
                    }
                }
            }

            switch (_colorMode)
            {
                case ColorMode.NOISE_MAP:
                    _noiseMapView.SetRendererTexture(TextureGenerator.TextureFromHeightMap(noiseMap, _heightMapColorA, _heightMapColorB, FilterMode.Bilinear, TextureWrapMode.Clamp, _heightMapLerpCurve));
                    break;
                case ColorMode.COLOR_MAP:
                    _noiseMapView.SetRendererTexture(TextureGenerator.TextureFromColorMap(colorMap, _size.x, _size.y, _filterMode, TextureWrapMode.Clamp));
                    break;
                default:
                    Debug.LogError($"Unhandled ColorMode {_colorMode}.", gameObject);
                    break;
            }
        }

        public void AddOffset(Vector2 offset)
        {
            _offset += offset;
        }

        public void AddOffset(float x, float y)
        {
            _offset.x += x;
            _offset.y += y;
        }

        private void OnValidate()
        {
            _size.x = Mathf.Max(_size.x, 1);
            _size.y = Mathf.Max(_size.y, 1);

            GenerateMap();
        }
    }
}