namespace Flow
{
    using System;
    using IoC;

    public readonly struct Path: IParsable<Path>
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
            return new Path(fileName);
        }

        public override string ToString() => Value;

        Path IParsable<Path>.Parse(string text) =>
            text ?? throw new ArgumentNullException(nameof(text));

        public bool IsEmpty => string.IsNullOrWhiteSpace(Value);
    }
}