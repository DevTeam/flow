namespace Flow
{
    using System;
    using IoC;

    public readonly struct CommandLineArgument: IParsable<CommandLineArgument>
    {
        [NotNull] public readonly string Value;

        public CommandLineArgument([NotNull] string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException(nameof(value));
            Value = value;
        }

        public static implicit operator CommandLineArgument([NotNull] string argument)
        {
            if (argument == null) throw new ArgumentNullException(nameof(argument));
            return new CommandLineArgument(argument);
        }

        public override string ToString() => Value;

        CommandLineArgument IParsable<CommandLineArgument>.Parse(string text) =>
            text ?? throw new ArgumentNullException(nameof(text));
    }
}