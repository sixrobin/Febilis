namespace RSLib.Extensions
{
    public static class StringExtensions
    {
        #region CONVERSION

        /// <summary>
        /// Tries to convert string to the specified Enum type.
        /// </summary>
        /// <typeparam name="T">Enum type.</typeparam>
        /// <returns>Parsed string to Enum if valid, else throws an exception.</returns>
        public static T ToEnum<T>(this string str) where T : System.Enum
        {
            if (str != null && System.Enum.IsDefined(typeof(T), str))
                return (T)System.Enum.Parse(typeof(T), str);

            throw new System.Exception($"Could not parse string {str} to a valid {typeof(T).Name} enum value.");
        }

        /// <summary>
        /// Tries to parse the string to a float value.
        /// </summary>
        /// <returns>Parsed string if succeed, else throws an exception.</returns>
        public static float ToFloat(this string str)
        {
            if (float.TryParse(str, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float value))
                return value;

            throw new System.Exception($"Could not parse string {str} to a valid float value.");
        }

        /// <summary>
        /// Tries to parse the string to an int value.
        /// </summary>
        /// <returns>Parsed string if succeed, else throws an exception.</returns>
        public static int ToInt(this string str)
        {
            if (int.TryParse(str, out int value))
                return value;

            throw new System.Exception($"Could not parse string {str} to a valid int value.");
        }

        /// <summary>
        /// Gets a new color from a hexadecimal string.
        /// </summary>
        /// <param name="hex">Hexadecimal string (RRGGBB, RRGGBBAA, #RRGGBB, #RRGGBBAA).</param>
        public static UnityEngine.Color ToColorFromHex(this string hex)
        {
            hex = hex.Replace("#", "");
            if (hex.Length < 8)
                hex += "ff";

            string[] split = new string[4];
            try
            {
                split[0] = hex.Substring(0, 2);
                split[1] = hex.Substring(2, 2);
                split[2] = hex.Substring(4, 2);
                split[3] = hex.Substring(6, 2);

                return new UnityEngine.Color
                (
                    int.Parse(split[0], System.Globalization.NumberStyles.HexNumber) / 255f,
                    int.Parse(split[1], System.Globalization.NumberStyles.HexNumber) / 255f,
                    int.Parse(split[2], System.Globalization.NumberStyles.HexNumber) / 255f,
                    int.Parse(split[3], System.Globalization.NumberStyles.HexNumber) / 255f
                );
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError($"An exception has occured while trying to convert hexadecimal value {hex} to a color.\nException: {e.Message}");
                return UnityEngine.Color.white;
            }
        }

        #endregion // CONVERSION

        #region GENERAL

        /// <summary>
        /// Returns a list of all the occurrences indexes of a given character in a string.
        /// </summary>
        /// <param name="str">String to inspect.</param>
        /// <param name="c">Character to look for.</param>
        /// <returns>A list of the character occurrences indexes.</returns>
        public static System.Collections.Generic.List<int> AllIndexesOf(this string str, char c)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            System.Collections.Generic.List<int> indexes = new System.Collections.Generic.List<int>();
            for (int i = 0; i < str.Length; ++i)
                if (str[i] == c)
                    indexes.Add(i);

            return indexes;
        }

        /// <summary>
        /// Returns a list of all the occurrences indexes of a given string in a string.
        /// </summary>
        /// <param name="str">String to inspect.</param>
        /// <param name="value">String to look for.</param>
        /// <returns>A list of the string occurrences indexes.</returns>
        public static System.Collections.Generic.List<int> AllIndexesOf(this string str, string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            System.Collections.Generic.List<int> indexes = new System.Collections.Generic.List<int>();
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    return indexes;

                indexes.Add(index);
            }
        }

        /// <summary>
        /// Copies the string value to clipboard, using GUIUtility.systemCopyBuffer.
        /// </summary>
        public static void CopyToClipboard(this string str)
        {
            UnityEngine.GUIUtility.systemCopyBuffer = str;
        }

        /// <summary>
        /// Extracts the capital letters from a given string.
        /// </summary>
        /// <param name="str">String to extract capital letters of.</param>
        /// <param name="forceIncludeFirstChar">Should include the first string char event if not capital.</param>
        /// <returns>Capital letters of the string.</returns>
        public static string ExtractCapitalLetters(this string str, bool forceIncludeFirstChar = false)
        {
            string result = string.Empty;

            for (int i = 0; i < str.Length; ++i)
                if (i == 0 && forceIncludeFirstChar || char.IsUpper(str[i]))
                    result += str[i];

            return result;
        }

        /// <summary>
        /// Removes the string first character.
        /// </summary>
        /// <returns>String without its first character.</returns>
        public static string RemoveFirst(this string str)
        {
            return string.IsNullOrEmpty(str) ? str : str.Substring(1);
        }

        /// <summary>
        /// Removes as many chars as specified from the start of the string.
        /// </summary>
        /// <param name="str">String to remove first characters of.</param>
        /// <param name="amount">Amount of characters to remove.</param>
        /// <returns>String with removed characters.</returns>
        public static string RemoveFirst(this string str, int amount)
        {
            int amountClamped = amount < 1 ? 1 : amount > str.Length ? str.Length : amount;
            return string.IsNullOrEmpty(str) ? str : str.Substring(amountClamped);
        }

        /// <summary>
        /// Removes the first occurrence of the given string in the source string.
        /// </summary>
        /// <param name="str">String to remove another string occurrence from.</param>
        /// <param name="toRemove">String occurrence to remove if found.</param>
        /// <returns>String without the given string first occurrence if it has been found.</returns>
        public static string RemoveFirstOccurrence(this string str, string toRemove)
        {
            int index = str.IndexOf(toRemove);
            return (index < 0) ? str : str.Remove(index, toRemove.Length);
        }

        /// <summary>
        /// Removes the string last character.
        /// </summary>
        /// <returns>String without its last character.</returns>
        public static string RemoveLast(this string str)
        {
            return string.IsNullOrEmpty(str) ? str : str.Substring(0, str.Length - 1);
        }

        /// <summary>
        /// Removes as many chars as specified from the end of the string.
        /// </summary>
        /// <param name="str">String to remove last characters of.</param>
        /// <param name="amount">Amount of chars to remove.</param>
        /// <returns>String with removed characters.</returns>
        public static string RemoveLast(this string str, int amount)
        {
            int amountClamped = amount < 0 ? 0 : amount > str.Length ? str.Length : amount;
            return string.IsNullOrEmpty(str) ? str : str.Substring(0, str.Length - amountClamped);
        }

        /// <summary>
        /// Reverses the string.
        /// </summary>
        /// <returns>String reversed.</returns>
        public static string Reverse(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;

            int stringLength = str.Length;
            char[] reversed = new char[stringLength];

            for (int i = str.Length - 1; i >= 0; --i)
                reversed[stringLength - i - 1] = str[i];

            return new string(reversed);
        }

        /// <summary>
        /// Sets the string first letter to uppercase.
        /// </summary>
        /// <returns>String with first letter to uppercase.</returns>
        public static string UpperFirst(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            char[] copy = str.ToCharArray();
            copy[0] = char.ToUpper(copy[0]);
            return new string(copy);
        }

        /// <summary>
        /// Replaces every multiple spaces in given string by one space.
        /// </summary>
        /// <returns>String with first letter to uppercase.</returns>
        public static string ReplaceMultipleSpacesBySingleOne(this string str)
        {
            return System.Text.RegularExpressions.Regex.Replace(str, @"\s+", " ");
        }

        #endregion // GENERAL

        #region STYLES

        /// <summary>
        /// Adds bold tag to the given string.
        /// </summary>
        /// <param name="str">String to style as bold.</param>
        /// <returns>String with bold tag.</returns>
        public static string ToBold(this string str)
        {
            return $"<b>{str}</b>";
        }

        /// <summary>
        /// Adds bold tag to the given string if a condition is fulfilled.
        /// </summary>
        /// <param name="str">String to style as bold.</param>
        /// <param name="condition">Condition to apply bold style.</param>
        /// <returns>String with bold tag if condition is fulfilled.</returns>
        public static string ToBoldIf(this string str, bool condition)
        {
            return condition ? str.ToBold() : str;
        }

        /// <summary>
        /// Adds color tag to the given string.
        /// </summary>
        /// <param name="str">String to color.</param>
        /// <param name="color">Color to apply to the string.</param>
        /// <returns>String with color tag.</returns>
        public static string ToColored(this string str, UnityEngine.Color color)
        {
            return $"<color=#{UnityEngine.ColorUtility.ToHtmlStringRGB(color)}>{str}</color>";
        }

        /// <summary>
        /// Adds color tag to the given string if a condition is fulfilled.
        /// </summary>
        /// <param name="str">String to color.</param>
        /// <param name="color">Color to apply to the string.</param>
        /// <param name="condition">Condition to color the string.</param>
        /// <returns>String with color tag if condition is fulfilled.</returns>
        public static string ToColoredIf(this string str, UnityEngine.Color color, bool condition)
        {
            return condition ? str.ToColored(color) : str;
        }

        /// <summary>
        /// Adds bold italic to the given string.
        /// </summary>
        /// <param name="str">String to style as italic.</param>
        /// <returns>String with italic tag.</returns>
        public static string ToItalic(this string str)
        {
            return $"<i>{str}</i>";
        }

        /// <summary>
        /// Adds italic tag to the given string if a condition is fulfilled.
        /// </summary>
        /// <param name="str">String to style as italic.</param>
        /// <param name="condition">Condition to apply italic style.</param>
        /// <returns>String with italic tag if condition is fulfilled.</returns>
        public static string ToItalicIf(this string str, bool condition)
        {
            return condition ? str.ToItalic() : str;
        }

        #endregion // STYLES
    }
}