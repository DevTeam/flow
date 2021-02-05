namespace Flow
{
    using System;
    using IoC;

    public readonly struct EnvironmentVariable: IParsable<EnvironmentVariable>
    {
        [NotNull] public readonly string Name;
        [NotNull] public readonly string Value;

        public EnvironmentVariable([NotNull] string name, [NotNull] string value)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException(nameof(name));
            if (value == null) throw new ArgumentNullException(nameof(value));
            Name = name;
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public static implicit operator EnvironmentVariable([NotNull] string variable)
        {
            if (variable == null) throw new ArgumentNullException(nameof(variable));
            var parts = variable.Split('=');
            switch (parts.Length)
            {
                case 1:
                    return new EnvironmentVariable(variable, string.Empty);

                case 2:
                    return new EnvironmentVariable(parts[0].Trim(), parts[1].Trim());

                default:
                    throw new InvalidCastException();
            }
        }

        public override string ToString() => $"{Name}={Value}";

        EnvironmentVariable IParsable<EnvironmentVariable>.Parse(string text) =>
            text ?? throw new ArgumentNullException(nameof(text));
    }
}