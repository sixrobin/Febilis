namespace Templar
{
    using System.Collections.Generic;
    using UnityEngine;

    public class TextureMaker : MonoBehaviour
    {
        [System.Serializable]
        public class ColorReplacement
        {
            public Color baseColor;
            public Color newColor;
        }

        [SerializeField] private int textureCreationCount = 100;
        [SerializeField] private UnityEngine.UI.Image _image = null;
        [SerializeField] private ColorReplacement[] _replacements = null;

        private Dictionary<Color, Color> _palette;

        private void CreateTexture()
        {
            Texture2D initTex = _image.sprite.texture;
            int w = initTex.width;
            int h = initTex.height;

            Texture2D newTex = new Texture2D(w, h, initTex.format, false)
            {
                name = $"{initTex.name}_copy",
                filterMode = FilterMode.Point,
                wrapMode = initTex.wrapMode
            };

            for (int x = 0; x < w; ++x)
            {
                for (int y = 0; y < h; ++y)
                {
                    Color sourceColor = initTex.GetPixel(x, y);
                    Color? targetColor = null;

                    foreach (KeyValuePair<Color, Color> replace in _palette)
                    {
                        if (Mathf.Approximately(sourceColor.r, replace.Key.r)
                            && Mathf.Approximately(sourceColor.g, replace.Key.g)
                            && Mathf.Approximately(sourceColor.b, replace.Key.b))
                        {
                            targetColor = replace.Value;
                            break;
                        }
                    }

                    newTex.SetPixel(x, y, targetColor ?? Color.red);
                }
            }

            newTex.Apply();

            _image.overrideSprite = Sprite.Create(
                newTex,
                new Rect(0, 0, newTex.width, newTex.height),
                _image.sprite.pivot,
                _image.sprite.pixelsPerUnit);
        }

        private void Start()
        {
            _palette = new Dictionary<Color, Color>();
            foreach (ColorReplacement replacement in _replacements)
                _palette.Add(replacement.baseColor, replacement.newColor);

            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

            for (int i = 0; i < textureCreationCount; ++i)
                CreateTexture();

            sw.Stop();
            Debug.Log($"{sw.ElapsedMilliseconds} ms.");
        }
    }
}