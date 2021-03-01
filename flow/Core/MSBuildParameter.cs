namespace Flow.Core
{
    using System;
    using IoC;

    internal struct MSBuildParameter
    {
        public readonly string Name;
        public readonly string Value;

        public MSBuildParameter([NotNull] string name, [NotNull] string value)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));
            Name = name;
            Value = value;
        }
    }
}
