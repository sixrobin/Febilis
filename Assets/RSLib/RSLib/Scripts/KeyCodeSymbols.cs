namespace RSLib
{
    using UnityEngine;

    public static class KeyCodeSymbols
    {
        private static readonly System.Collections.Generic.Dictionary<KeyCode, string> s_symbols = new System.Collections.Generic.Dictionary<KeyCode, string>(new RSLib.Framework.Comparers.EnumComparer<KeyCode>())
        {
            { KeyCode.Ampersand, "&" },
            { KeyCode.Asterisk, "*" },
            { KeyCode.At, "@" },
            { KeyCode.BackQuote, "`" },
            { KeyCode.Backslash, "\\" },
            { KeyCode.Caret, "^" },
            { KeyCode.Colon, ":" },
            { KeyCode.Comma, "," },
            { KeyCode.Dollar, "$" },
            { KeyCode.DoubleQuote, "\"" },
            { KeyCode.Equals, "=" },
            { KeyCode.Exclaim, "!" },
            { KeyCode.Greater, ">" },
            { KeyCode.Hash, "#" },
            { KeyCode.KeypadDivide, "/" },
            { KeyCode.KeypadMinus, "-" },
            { KeyCode.KeypadMultiply, "*" },
            { KeyCode.KeypadPlus, "+" },
            { KeyCode.LeftBracket, "[" },
            { KeyCode.LeftCurlyBracket, "{" },
            { KeyCode.LeftParen, "(" },
            { KeyCode.Less, "<" },
            { KeyCode.Minus, "-" },
            { KeyCode.Percent, "%" },
            { KeyCode.Period, "." },
            { KeyCode.Pipe, "|" },
            { KeyCode.Plus, "+" },
            { KeyCode.Question, "?" },
            { KeyCode.Quote, "'" },
            { KeyCode.RightBracket, "]" },
            { KeyCode.RightCurlyBracket, "}" },
            { KeyCode.RightParen, ")" },
            { KeyCode.Semicolon, ";" },
            { KeyCode.Slash, "/" },
            { KeyCode.Tilde, "~" },
            { KeyCode.Underscore, "_" },
            { KeyCode.A, "A" },
            { KeyCode.B, "B" },
            { KeyCode.C, "C" },
            { KeyCode.D, "D" },
            { KeyCode.E, "E" },
            { KeyCode.F, "F" },
            { KeyCode.G, "G" },
            { KeyCode.H, "H" },
            { KeyCode.I, "I" },
            { KeyCode.J, "J" },
            { KeyCode.K, "K" },
            { KeyCode.L, "L" },
            { KeyCode.M, "M" },
            { KeyCode.N, "N" },
            { KeyCode.O, "O" },
            { KeyCode.P, "P" },
            { KeyCode.Q, "Q" },
            { KeyCode.R, "R" },
            { KeyCode.S, "S" },
            { KeyCode.T, "T" },
            { KeyCode.U, "U" },
            { KeyCode.V, "V" },
            { KeyCode.W, "W" },
            { KeyCode.X, "X" },
            { KeyCode.Y, "Y" },
            { KeyCode.Z, "Z" },
        };

        /// <summary>
        /// Gets the KeyCode symbol, or the KeyCode itself if no symbol is related.
        /// </summary>
        /// <param name="keyCode">KeyCode to get the symbol of.</param>
        /// <returns>The symbol if found, else the KeyCode to string.</returns>
        public static string GetSymbol(KeyCode keyCode)
        {
            return s_symbols.TryGetValue(keyCode, out string shortName) ? shortName : keyCode.ToString();
        }
    }
}