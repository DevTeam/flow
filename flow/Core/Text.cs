namespace Flow.Core
{
    using System;
    using IoC;

    internal readonly struct Text
    {
        [CanBeNull] public readonly string Value;
        public readonly Color Color;

        public Text([CanBeNull] string value, Color color = Color.Default)
        {
            Value = value;
            Color = color;
        }

        public static implicit operator Text[](Text text) => new[] {text};

        public static implicit operator Text(string message) => new Text(message);

        public static Text[] operator + (Text text1, Text text2) => new[] { text1, text2 };

        public static Text[] operator +(Text[] text, Text text2)
        {
            var newText = new Text[text.Length + 1];
            Array.Copy(text, 0, newText, 0, text.Length);
            newText[text.Length] = text2;
            return newText;
        }

        public static Text[] operator +(Text text1, Text[] text)
        {
            var newText = new Text[text.Length + 1];
            newText[0] = text1;
            Array.Copy(text, 0, newText, 1, text.Length);
            return newText;
        }
    }
}
