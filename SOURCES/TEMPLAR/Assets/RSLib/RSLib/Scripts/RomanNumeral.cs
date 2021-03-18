namespace RSLib
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public static class RomanNumeralConverter
    {
        private const int NB_OF_ROMAN_NUMERAL_MAPS = 13;
        private const int MAX_ROMAN_NUMERAL_LENGTH = 15;
        private const string INVALID_STR_ON_WRONG_VALUE = "INVALID_ROMAN_NUMERAL";

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

        private static readonly Regex validRomanNumeralRegex =
            new Regex("^(?i:(?=[MDCLXVI])((M{0,3})((C[DM])|(D?C{0,3}))?((X[LC])|(L?XX{0,2})|L)?((I[VX])|(V?(II{0,2}))|V)?))$", RegexOptions.Compiled);
        
        /// <summary>Checks if a string format is a valid roman numeral.</summary>
        /// <returns>True if it is valid, else false.</returns>
        public static bool IsValidRomanNumeral(this string str)
        {
            return validRomanNumeralRegex.IsMatch(str);
        }

        /// <summary>Parses a roman numeral string to its corresponding value. Returns -1 if string is not valid.</summary>
        /// <returns>Parsed string if it is valid, else -1.</returns>
        public static int ParseRomanNumeralToInt(this string str)
        {
            if (str == null)
                return -1;

            str = str.ToUpperInvariant().Trim();

            int length = str.Length;

            if (length == 0 || !str.IsValidRomanNumeral())
                return -1;

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

        /// <summary>Parses an integer value to its corresponding roman numeral string.
        /// Maximum number is 3999 due to the non-existence of a symbol for 5000.</summary>
        /// <returns>Roman numeral string.</returns>
        public static string ToRomanNumeralString(this int i)
        {
            const int MinValue = 1;
            const int MaxValue = 3999;

            if (i < MinValue || i > MaxValue)
                return INVALID_STR_ON_WRONG_VALUE;

            System.Text.StringBuilder sb = new System.Text.StringBuilder(MAX_ROMAN_NUMERAL_LENGTH);

            foreach (KeyValuePair<string, int> pair in s_romanNumerals)
            {
                while (i / pair.Value > 0)
                {
                    sb.Append(pair.Key);
                    i -= pair.Value;
                }
            }

            return sb.ToString();
        }
    }
}