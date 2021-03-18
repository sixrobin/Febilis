namespace RSLib.Extensions
{
    using UnityEngine;

    public static class Texture2DExtensions
    {
        #region GENERAL

        /// <summary>
        /// Creates a new Texture2D that is a x-flipped version of the original.
        /// Read/Write must be enabled on the reference texture asset.
        /// </summary>
        /// <returns>Flipped texture.</returns>
        public static Texture2D FlipX(this Texture2D original)
        {
            int w = original.width;
            int h = original.height;

            Texture2D flipped = new Texture2D(w, h)
            {
                wrapModeU = TextureWrapMode.Clamp
            };

            for (int x = 0; x < w; ++x)
                for (int y = 0; y < h; ++y)
                    flipped.SetPixel(w - x - 1, y, original.GetPixel(x, y));

            flipped.Apply();
            return flipped;
        }

        #endregion
    }
}