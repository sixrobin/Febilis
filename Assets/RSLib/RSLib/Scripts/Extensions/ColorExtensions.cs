namespace RSLib.Extensions
{
    using UnityEngine;

    public static class ColorExtensions
    {
        #region BLEND

        /// <summary>
        /// Blends a color with as many colors as wanted, all with the same weight.
        /// </summary>
        /// <param name="color">Source color to blend with others.</param>
        /// <param name="colorsToBlend">Colors to blend with the base color.</param>
        /// <returns>Blended color.</returns>
        public static Color BlendWith(this Color color, params Color[] colorsToBlend)
        {
            Color blendedColor = color;
            for (int i = 0, weight = 2; i < colorsToBlend.Length; ++i, ++weight)
                blendedColor = Color.Lerp(blendedColor, colorsToBlend[i], 1f / weight);

            return blendedColor;
        }

        /// <summary>
        /// Blends all colors with the same weight.
        /// </summary>
        /// <param name="colorsToBlend">Colors to blend.</param>
        /// <returns>Blended color.</returns>
        public static Color BlendColors(params Color[] colorsToBlend)
        {
            Color blendedColor = colorsToBlend[0];
            for (int i = 1, weight = 2; i < colorsToBlend.Length; ++i, ++weight)
                blendedColor = Color.Lerp(blendedColor, colorsToBlend[i], 1f / weight);

            return blendedColor;
        }

        #endregion // BLEND

        #region CONVERSION

        /// <summary>
        /// Converts color into a hexadecimal value to string.
        /// </summary>
        /// <returns>Color to string with format RRGGBB.</returns>
        public static string ToHexRGB(this Color color)
        {
            return ColorUtility.ToHtmlStringRGB(color);
        }

        /// <summary>
        /// Converts color into a hexadecimal value to string.
        /// </summary>
        /// <returns>Color to string with format RRGGBBAA.</returns>
        public static string ToHexRGBA(this Color color)
        {
            return ColorUtility.ToHtmlStringRGBA(color);
        }

        #endregion // CONVERSION

        #region GENERAL

        /// <summary>
        /// Gets a color's copy with the RGB values modified without modifying the alpha.
        /// </summary>
        /// <param name="color">Source color.</param>
        /// <param name="r">New red value.</param>
        /// <param name="g">New green value.</param>
        /// <param name="b">New blue value.</param>
        public static Color SetRGB(this Color color, float r, float g, float b)
        {
            return color.WithR(r).WithG(g).WithB(b);
        }

        /// <summary>
        /// Gets a color's copy with the RGB values modified without modifying the alpha.
        /// </summary>
        /// <param name="color">Source color.</param>
        /// <param name="copy">Color to copy the RGB channels of.</param>
        public static Color SetRGB(this Color color, Color copy)
        {
            return color.WithR(copy.r).WithG(copy.g).WithB(copy.b);
        }

        #endregion // GENERAL

        #region WITH

        /// <summary>
        /// Gets a color's copy with new red value.
        /// </summary>
        /// <param name="color">Source color.</param>
        /// <param name="r">New red value.</param>
        public static Color WithR(this Color color, float r)
        {
            return new Color(r, color.g, color.b, color.a);
        }

        /// <summary>
        /// Gets a color's copy with new green value.
        /// </summary>
        /// <param name="color">Source color.</param>
        /// <param name="g">New green value.</param>
        public static Color WithG(this Color color, float g)
        {
            return new Color(color.r, g, color.b, color.a);
        }

        /// <summary>
        /// Gets a color's copy with new blue value.
        /// </summary>
        /// <param name="color">Source color.</param>
        /// <param name="b">New blue value.</param>
        public static Color WithB(this Color color, float b)
        {
            return new Color(color.r, color.g, b, color.a);
        }

        /// <summary>
        /// Gets a color's copy with new alpha value.
        /// </summary>
        /// <param name="color">Source color.</param>
        /// <param name="a">New alpha value.</param>
        public static Color WithA(this Color color, float a)
        {
            return new Color(color.r, color.g, color.b, a);
        }

        #endregion // WITH
    }
}