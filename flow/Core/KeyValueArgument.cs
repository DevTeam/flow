namespace Flow.Core
{
    using System;
    using IoC;

    internal readonly struct KeyValueArgument
    {
        public readonly string Name;
        public readonly string Value;

        public KeyValueArgument([NotNull] string name, [NotNull] string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Value = value;
        }

        public override string ToString() => $"Name: \"{Name}\",  Value: \"{Value}\"";
    }
}
