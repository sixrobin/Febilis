namespace RSLib
{
    using RSLib.Maths;
    using UnityEngine;

    public static class TextureGenerator
    {
        public static Texture2D TextureFromColorMap(Color[] colorMap, int w, int h, FilterMode filterMode, TextureWrapMode wrapMode)
        {
            Texture2D texture = new Texture2D(w, h)
            {
                filterMode = filterMode,
                wrapMode = wrapMode
            };

            texture.SetPixels(colorMap);
            texture.Apply();

            return texture;
        }

        public static Texture2D TextureFromHeightMap(float[,] heightMap, Color colorA, Color colorB, FilterMode filterMode, TextureWrapMode wrapMode, Maths.Curve lerpCurve = Maths.Curve.Linear)
        {
            int w = heightMap.GetLength(0);
            int h = heightMap.GetLength(1);

            Color[] colorMap = new Color[w * h];
            for (int x = 0; x < w; ++x)
                for (int y = 0; y < h; ++y)
                    colorMap[x + h * y] = Color.Lerp(colorA, colorB, heightMap[x, y].Ease(lerpCurve));

            return TextureFromColorMap(colorMap, w, h, filterMode, wrapMode);
        }
    }
}