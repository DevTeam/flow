namespace Flow
{
    using System;
    using System.Collections.Generic;
    using Core;
    using IoC;

    public readonly struct CommandLineArgument: IFromText<CommandLineArgument>
    {
        [NotNull] public readonly string Value;
        internal readonly CommandLineArgumentType Type;

        public CommandLineArgument([NotNull] string value)
        : this(value, CommandLineArgumentType.Default)
        {
        }

        internal CommandLineArgument([NotNull] string value, CommandLineArgumentType type)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException(nameof(value));
            Value = value;
            Type = type;
        }

        public static implicit operator CommandLineArgument([NotNull] string argument)
        {
            if (argument == null) throw new ArgumentNullException(nameof(argument));
            using (var enumerator = argument.GetEnumerator())
            {
                return new CommandLineArgument(enumerator.Parse());
            }
        }

        public override string ToString() => Value;

        CommandLineArgument IFromText<CommandLineArgument>.Parse(IEnumerator<char> text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            return new CommandLineArgument(text.Parse());
        }
    }
}