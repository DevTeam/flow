﻿namespace Flow.Core
{
    using System.Collections.Generic;
    using System.Linq;

    // ReSharper disable once InconsistentNaming
    internal class MSBuildParameterToStringConverter: IConverter<MSBuildParameter, string>
    {
        public string Convert(MSBuildParameter parameter) =>
            $"/p:{NormalizeName(parameter.Name)}={NormalizeValue(parameter.Value, false)}";

        internal string NormalizeName(string name) => 
            new string(name.Select((c, index) => (index == 0 ? IsValidInitialElementNameCharacter(c) : IsValidSubsequentElementNameCharacter(c)) ? c : '_').ToArray());

        internal string NormalizeValue(string value, bool isCommandLineParameter)
        {
            var str = new string(EscapeSymbols(value, isCommandLineParameter).ToArray());
            if (string.IsNullOrWhiteSpace(str) || str.Contains(';'))
            {
                return $"\"{str}\"";
            }

            return str;
        }

        private bool IsValidInitialElementNameCharacter(char c) =>
            (c >= 'A' && c <= 'Z') ||
            (c >= 'a' && c <= 'z') ||
            (c == '_');

        private bool IsValidSubsequentElementNameCharacter(char c) =>
            (c >= 'A' && c <= 'Z') ||
            (c >= 'a' && c <= 'z') ||
            (c >= '0' && c <= '9') ||
            (c == '_') ||
            (c == '-');

        private static IEnumerable<char> EscapeSymbols(IEnumerable<char> chars, bool isCommandLineParameter)
        { 
            foreach (var ch in chars)
            {
                if (char.IsLetterOrDigit(ch) || (ch == ';' && !isCommandLineParameter) || ch == '%')
                {
                    yield return ch;
                }
                else
                {
                    yield return '%';
                    foreach (var c in $"{(byte)ch:X02}")
                    {
                        yield return c;
                    }
                }
            }
        }
    }
}