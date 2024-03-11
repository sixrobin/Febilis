namespace RSLib.Noise
{
    public static class Noise
    {
        public static float[,] GenerateNoiseMap(UnityEngine.Vector2Int size, float scale, int seed, int octaves, float persistance, float lacunarity, UnityEngine.Vector2 offset)
        {
            return GenerateNoiseMap(size.x, size.y, scale, seed, octaves, persistance, lacunarity, offset);
        }

        public static float[,] GenerateNoiseMap(int w, int h, float scale, int seed, int octaves, float persistance, float lacunarity, UnityEngine.Vector2 offset)
        {
            float[,] noiseMap = new float[w, h];

            System.Random rnd = new System.Random(seed);
            UnityEngine.Vector2[] octaveOffsets = new UnityEngine.Vector2[octaves];
            for (int i = 0; i < octaves; ++i)
                octaveOffsets[i] = new UnityEngine.Vector2(rnd.Next(-10000, 10000) + offset.x, rnd.Next(-10000, 10000) + offset.y);

            if (scale < 0.0001f)
                scale = 0.0001f;

            float maxNoiseHeight = float.MinValue;
            float minNoiseHeight = float.MaxValue;

            float halfWidth = w * 0.5f;
            float halfHeight = h * 0.5f;

            for (int x = 0; x < w; ++x)
            {
                for (int y = 0; y < h; ++y)
                {
                    float amplitude = 1f;
                    float frequency = 1f;
                    float noiseHeight = 0f;

                    for (int i = 0; i < octaves; ++i)
                    {
                        float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
                        float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;

                        float perlin = UnityEngine.Mathf.PerlinNoise(sampleX, sampleY) * 2f - 1f;

                        noiseHeight += perlin * amplitude;
                        amplitude *= persistance;
                        frequency *= lacunarity;
                    }

                    if (noiseHeight > maxNoiseHeight)
                        maxNoiseHeight = noiseHeight;
                    else if (noiseHeight < minNoiseHeight)
                        minNoiseHeight = noiseHeight;

                    noiseMap[x, y] = noiseHeight;
                }
            }

            for (int x = 0; x < w; ++x)
                for (int y = 0; y < h; ++y)
                    noiseMap[x, y] = UnityEngine.Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);

            return noiseMap;
        }
    }
}