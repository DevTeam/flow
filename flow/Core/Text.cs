namespace Flow.Core
{
    using System;
    using System.Collections.Generic;
    using IoC;

    internal readonly struct Text : IFromText<Text>
    {
        [NotNull] public readonly string Value;
        public readonly Color Color;

        public static readonly Text NewLine = new Text(Environment.NewLine);
        public static readonly Text Tab = "Default:    ";

        public Text([NotNull] string value, Color color = Color.Default)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            Color = color;
        }

        public static implicit operator Text[](Text text) => new[] {text};

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

        public static implicit operator Text([NotNull] string variable)
        {
            if (variable == null) throw new ArgumentNullException(nameof(variable));
            using (var enumerator = variable.GetEnumerator())
            {
                var (colorStr, value) = enumerator.ParseTuple(':');
                if (!Enum.TryParse<Color>(colorStr, true, out var color))
                {
                    color = Color.Default;
                }

                return new Text(value, color);
            }
        }

        public override string ToString() => $"{Color}:{Value}";

        Text IFromText<Text>.Parse(IEnumerator<char> text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            var (colorStr, value) = text.ParseTuple(':');
            if (!Enum.TryParse<Color>(colorStr, true, out var color))
            {
                color = Color.Default;
            }

            return new Text(value, color);
        }
    }
}
