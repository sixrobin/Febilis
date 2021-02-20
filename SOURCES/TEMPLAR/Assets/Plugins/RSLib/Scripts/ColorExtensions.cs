namespace RSLib.Extensions
{
    using UnityEngine;

    public static class ColorExtensions
    {
        #region BLEND

        /// <summary>Blends a color with as many colors as wanted, all with the same weight.</summary>
        /// <param name="colorsToBlend">Colors to blend with the base color.</param>
        /// <returns>Blended color.</returns>
        public static Color BlendWith(this Color color, params Color[] colorsToBlend)
        {
            Color blendedColor = color;
            for (int i = 0, weight = 2; i < colorsToBlend.Length; ++i, ++weight)
                blendedColor = Color.Lerp(blendedColor, colorsToBlend[i], 1f / weight);

            return blendedColor;
        }

        /// <summary>Blends all colors with the same weight.</summary>
        /// <param name="colorsToBlend">Colors to blend.</param>
        /// <returns>Blended color.</returns>
        public static Color BlendColors(params Color[] colorsToBlend)
        {
            Color blendedColor = colorsToBlend[0];
            for (int i = 1, weight = 2; i < colorsToBlend.Length; ++i, ++weight)
                blendedColor = Color.Lerp(blendedColor, colorsToBlend[i], 1f / weight);

            return blendedColor;
        }

        #endregion

        #region GENERAL

        /// <summary>Gets a color's copy with the RGB values modified without modifying the alpha.</summary>
        /// <param name="r">New red value.</param>
        /// <param name="g">New green value.</param>
        /// <param name="b">New blue value.</param>
        public static Color SetRGB(this Color color, float r, float g, float b)
        {
            return color.WithR(r).WithG(g).WithB(b);
        }

        /// <summary>Gets a color's copy with the RGB values modified without modifying the alpha.</summary>
        /// <param name="copy">Color to copy the RGB channels of.</param>
        public static Color SetRGB(this Color c, Color copy)
        {
            return c.WithR(copy.r).WithG(copy.g).WithB(copy.b);
        }

        #endregion GENERAL

        #region WITH

        /// <summary>Gets a color's copy with new red value.</summary>
        /// <param name="r">New red value.</param>
        public static Color WithR(this Color c, float value)
        {
            return new Color(value, c.g, c.b, c.a);
        }

        /// <summary>Gets a color's copy with new green value.</summary>
        /// <param name="g"> ew green value.</param>
        public static Color WithG(this Color c, float value)
        {
            return new Color(c.r, value, c.b, c.a);
        }

        /// <summary>Gets a color's copy with new blue value.</summary>
        /// <param name="b">New blue value.</param>
        public static Color WithB(this Color c, float value)
        {
            return new Color(c.r, c.g, value, c.a);
        }

        /// <summary>Gets a color's copy with new alpha value.</summary>
        /// <param name="a">New alpha value.</param>
        public static Color WithA(this Color c, float value)
        {
            return new Color(c.r, c.g, c.b, value);
        }

        #endregion WITH
    }
}