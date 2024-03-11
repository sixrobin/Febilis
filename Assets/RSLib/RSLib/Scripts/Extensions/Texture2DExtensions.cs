namespace RSLib.Extensions
{
    using UnityEngine;

    public static class Texture2DExtensions
    {
        #region FLIP

        /// <summary>
        /// Creates a new Texture2D that is a X flipped version of the original.
        /// Read/Write must be enabled on the reference texture asset.
        /// </summary>
        /// <returns>Flipped texture in the X axis.</returns>
        public static Texture2D FlipX(this Texture2D original)
        {
            UnityEngine.Assertions.Assert.IsTrue(
                original.isReadable,
                $"Cannot flip Texture2D {original.name} on X since Read/Write has not been checked.");

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

        /// <summary>
        /// Creates a new Texture2D that is a Y flipped version of the original.
        /// Read/Write must be enabled on the reference texture asset.
        /// </summary>
        /// <returns>Flipped texture in the Y axis.</returns>
        public static Texture2D FlipY(this Texture2D original)
        {
            UnityEngine.Assertions.Assert.IsTrue(
                original.isReadable,
                $"Cannot flip Texture2D {original.name} on Y since Read/Write has not been checked.");

            int w = original.width;
            int h = original.height;

            Texture2D flipped = new Texture2D(w, h)
            {
                wrapModeU = TextureWrapMode.Clamp
            };

            for (int x = 0; x < w; ++x)
                for (int y = 0; y < h; ++y)
                    flipped.SetPixel(x, h - y - 1, original.GetPixel(x, y));

            flipped.Apply();
            return flipped;
        }

        /// <summary>
        /// Creates a new Texture2D that is a XY flipped version of the original.
        /// Read/Write must be enabled on the reference texture asset.
        /// </summary>
        /// <returns>Flipped texture in the X and Y axis.</returns>
        public static Texture2D FlipXY(this Texture2D original)
        {
            return original.FlipX().FlipY();
        }

        #endregion // FLIP
    }
}