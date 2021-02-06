namespace Flow
{
    using System;
    using System.Collections.Generic;
    using IoC;
    using Services;

    public readonly struct EnvironmentVariable: IFromText<EnvironmentVariable>
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
            using (var enumerator = variable.GetEnumerator())
            {
                var (name, value) = enumerator.ParseTuple();
                return new EnvironmentVariable(name, value);
            }
        }

        public override string ToString() => $"{Name}={Value}";

        EnvironmentVariable IFromText<EnvironmentVariable>.Parse(IEnumerator<char> text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            var (name, value) = text.ParseTuple();
            return new EnvironmentVariable(name, value);
        }
    }
}