﻿namespace Templar
{
    using UnityEngine;

    public static class KeyCodeShortNames
    {
        private static readonly System.Collections.Generic.Dictionary<KeyCode, string> s_shortNames = new System.Collections.Generic.Dictionary<KeyCode, string>(new RSLib.Framework.Comparers.EnumComparer<KeyCode>())
        {
            { KeyCode.Escape, "Esc" },
            { KeyCode.LeftAlt, "Alt L" },
            { KeyCode.RightAlt, "Alt R" },
            { KeyCode.LeftControl, "Ctrl L" },
            { KeyCode.RightControl, "Ctrl R" },
            { KeyCode.LeftShift, "Shift L" },
            { KeyCode.RightShift, "Shift R" },
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
        };

        public static string GetShortName(KeyCode keyCode)
        {
            return s_shortNames.TryGetValue(keyCode, out string shortName) ? shortName : keyCode.ToString();
        }
    }
}