namespace RSLib.Extensions
{
    public static class IntExtensions
    {
        private const string ZERO_STR = "0";

        #region GENERAL

        /// <summary>
        /// Adds leading zeros before the number.
        /// </summary>
        /// <param name="value">Source value to add leading zeros to.</param>
        /// <param name="outputLength">Number of zeros to add.</param>
        /// <returns>The number as a string, with leading zeros.</returns>
        public static string AddLeading0(this int value, int outputLength)
        {
            int sign = System.Math.Sign(value);
            value = System.Math.Abs(value);

            string valueStr = string.Empty;
            int compNumber = (int)System.Math.Pow(10f, --outputLength);

            while (compNumber > 1)
            {
                if (value < compNumber)
                    valueStr += ZERO_STR;

                compNumber = (int)System.Math.Pow(10f, --outputLength);
            }

            return $"{(sign < 0 ? "-" : string.Empty)}{valueStr + value}";
        }

        #endregion // GENERAL
    }
}