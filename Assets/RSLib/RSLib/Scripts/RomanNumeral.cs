namespace RSLib
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public static class RomanNumeralConverter
    {
        private const int MIN_VALUE = 1;
        private const int MAX_VALUE = 3999;
        private const int NB_OF_ROMAN_NUMERAL_MAPS = 13;
        private const int MAX_ROMAN_NUMERAL_LENGTH = 15;

        private static readonly Dictionary<string, int> s_romanNumerals = new Dictionary<string, int>(NB_OF_ROMAN_NUMERAL_MAPS)
        {
            { "M", 1000 },
            { "CM", 900 },
            { "D", 500 },
            { "CD", 400 },
            { "C", 100 },
            { "XC", 90 },
            { "L", 50 },
            { "XL", 40 },
            { "X", 10 },
            { "IX", 9 },
            { "V", 5 },
            { "IV", 4 },
            { "I", 1 }
        };

        private static readonly Regex s_validRomanNumeralRegex = new Regex(
            "^(?i:(?=[MDCLXVI])((M{0,3})((C[DM])|(D?C{0,3}))?((X[LC])|(L?XX{0,2})|L)?((I[VX])|(V?(II{0,2}))|V)?))$",
            RegexOptions.Compiled);
        
        /// <summary>
        /// Checks if a string format is a valid roman numeral.
        /// </summary>
        /// <returns>True if it is valid, else false.</returns>
        public static bool IsValidRomanNumeral(this string str)
        {
            return s_validRomanNumeralRegex.IsMatch(str);
        }

        /// <summary>
        /// Parses a roman numeral string to its corresponding value.
        /// </summary>
        /// <returns>Parsed string if it is valid, else throws an exception.</returns>
        public static int ParseRomanNumeralToInt(this string str)
        {
            if (string.IsNullOrEmpty(str))
                throw new System.Exception("Cannot parse a null or empty string from a roman numeral value to an int value.");

            str = str.ToUpperInvariant().Trim();

            int length = str.Length;

            if (!str.IsValidRomanNumeral())
                throw new System.Exception($"String {str} has an invalid format to be parsed from a roman numeral value to an int value.");

            int total = 0;
            int strLength = length;

            while (strLength > 0)
            {
                int digit = s_romanNumerals[str[--strLength].ToString()];

                if (strLength > 0)
                {
                    int previousDigit = s_romanNumerals[str[strLength - 1].ToString()];
                    if (previousDigit < digit)
                    {
                        digit -= previousDigit;
                        strLength--;
                    }
                }

                total += digit;
            }

            return total;
        }

        /// <summary>
        /// Parses an integer value to its corresponding roman numeral string.
        /// Maximum number is 3999 due to the non-existence of a symbol for 5000.
        /// </summary>
        /// <returns>Roman numeral string.</returns>
        public static string ToRomanNumeralString(this int i)
        {
            if (i < MIN_VALUE || i > MAX_VALUE)
                throw new System.Exception($"{i} is less than 1 or higher than 3999, and thus cannot be parsed as a roman numeral value.");

            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder(MAX_ROMAN_NUMERAL_LENGTH);

            foreach (KeyValuePair<string, int> pair in s_romanNumerals)
            {
                while (i / pair.Value > 0)
                {
                    stringBuilder.Append(pair.Key);
                    i -= pair.Value;
                }
            }

            return stringBuilder.ToString();
        }
    }
}