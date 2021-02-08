namespace Flow
{
    using System;
    using System.Collections.Generic;
    using Core;
    using IoC;

    public readonly struct Path: IFromText<Path>
    {
        [NotNull] public readonly string Value;

        public Path([NotNull] string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException(nameof(value));
            Value = value;
        }

        public static implicit operator Path ([NotNull] string fileName)
        {
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));
            using (var enumerator = fileName.GetEnumerator())
            {
                return new Path(enumerator.Parse());
            }
        }

        public override string ToString() => Value;

        Path IFromText<Path>.Parse(IEnumerator<char> text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            return new Path(text.Parse());
        }

        public bool IsEmpty => string.IsNullOrWhiteSpace(Value);
    }
}