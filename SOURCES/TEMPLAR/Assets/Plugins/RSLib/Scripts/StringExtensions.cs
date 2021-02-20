namespace RSLib.Extensions
{
    using RSLib.Maths;

    public static class StringExtensions
    {
        #region CONVERSION

        /// <summary>Tries to convert string to the specified Enum type.</summary>
        /// <typeparam name="T">Enum type.</typeparam>
        /// <returns>Parsed string to Enum if valid, else Enum default value.</returns>
        public static T ToEnum<T>(this string str) where T : System.Enum
        {
            if (str == null || !System.Enum.IsDefined(typeof(T), str))
                return default;

            return (T)System.Enum.Parse(typeof(T), str);
        }

        /// <summary>Tries to parse the string to a float and returns -1f if parsing was invalid.</summary>
        /// <returns>Successfully parsed string or -1f.</returns>
        public static float ToFloat(this string str)
        {
            return float.TryParse(str, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float result)
                ? result
                : -1f;
        }

        /// <summary>Tries to parse the string to an int and returns -1 if parsing was invalid.</summary>
        /// <returns>Successfully parsed string or -1.</returns>
        public static int ToInt(this string str)
        {
            return int.TryParse(str, out int result)
                ? result
                : -1;
        }

        #endregion CONVERSION

        #region GENERAL

        /// <summary>Removes the string first character.</summary>
        /// <returns>String without its first character.</returns>
        public static string RemoveFirst(this string str)
        {
            return string.IsNullOrEmpty(str) ? str : str.Substring(1);
        }

        /// <summary>Removes as many chars as specified from the start of the string.</summary>
        /// <param name="amount">Amount of chars to remove.</param>
        /// <returns>String without the removed characters.</returns>
        public static string RemoveFirst(this string str, int amount)
        {
            return string.IsNullOrEmpty(str) ? str : str.Substring(amount.Clamp(1, str.Length));
        }

        /// <summary>Removes the first occurrence of the given string in the source string.</summary>
        /// <returns>String without the given string first occurence if it has been found.</returns>
        public static string RemoveFirstOccurrence(this string str, string toRemove)
        {
            int index = str.IndexOf(toRemove);
            return (index < 0) ? str : str.Remove(index, toRemove.Length);
        }

        /// <summary>Removes the string last character.</summary>
        /// <returns>String without its last character.</returns>
        public static string RemoveLast(this string str)
        {
            return string.IsNullOrEmpty(str) ? str : str.Substring(0, str.Length - 1);
        }

        /// <summary>Removes as many chars as specified from the end of the string.</summary>
        /// <param name="amount">Amount of chars to remove.</param>
        /// <returns>String without the removed characters.</returns>
        public static string RemoveLast(this string str, int amount)
        {
            return string.IsNullOrEmpty(str) ? str : str.Substring(0, str.Length - amount.Clamp(0, str.Length));
        }

        /// <summary>Reverses the string.</summary>
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

        /// <summary>Sets the string first letter to uppercase.</summary>
        /// <returns>String with first letter to uppercase.</returns>
        public static string UpperFirst(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            char[] copy = str.ToCharArray();
            copy[0] = char.ToUpper(copy[0]);
            return new string(copy);
        }

        #endregion GENERAL

        #region STYLES

        /// <summary>Adds bold tag to the given string.</summary>
        /// <returns>String with bold tag.</returns>
        public static string ToBold(this string str)
        {
            return $"<b>{str}</b>";
        }

        /// <summary>Adds bold tag to the given string if a condition is fulfilled.</summary>
        /// <returns>String with bold tag if condition is fulfilled.</returns>
        public static string ToBoldIf(this string str, bool condition)
        {
            return condition ? $"<b>{str}</b>" : str;
        }

        /// <summary>Adds color tag to the given string.</summary>
        /// <param name="color">Color to apply to the string.</param>
        /// <returns>String with color tag.</returns>
        public static string ToColored(this string str, UnityEngine.Color color)
        {
            return $"<color=#{UnityEngine.ColorUtility.ToHtmlStringRGB(color)}>{str}</color>";
        }

        /// <summary>Adds color tag to the given string if a condition is fulfilled.</summary>
        /// <param name="color">Color to apply to the string.</param>
        /// <param name="condition">Conditions to color the string.</param>
        /// <returns>String with color tag if condition is fulfilled.</returns>
        public static string ToColoredIf(this string str, UnityEngine.Color color, bool condition)
        {
            return condition ? $"<color=#{UnityEngine.ColorUtility.ToHtmlStringRGB(color)}>{str}</color>" : str;
        }

        /// <summary>Adds bold italic to the given string.</summary>
        /// <returns>String with italic tag.</returns>
        public static string ToItalic(this string str)
        {
            return $"<i>{str}</i>";
        }

        /// <summary>Adds italic tag to the given string if a condition is fulfilled.</summary>
        /// <returns>String with italic tag if condition is fulfilled.</returns>
        public static string ToItalicIf(this string str, bool condition)
        {
            return condition ? $"<i>{str}</i>" : str;
        }

        #endregion STYLES
    }
}