namespace Flow.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using IoC;
    using static Tags;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class ArgumentsToStringConverter : IConverter<IEnumerable<CommandLineArgument>, string>
    {
        [NotNull] private readonly IConverter<CommandLineArgument, string> _argumentToStringConverter;
        [NotNull] private readonly string _separator;

        public ArgumentsToStringConverter(
            [Tag(ArgumentsSeparatorChar)] char argumentsSeparator,
            [NotNull] IConverter<CommandLineArgument, string> argumentToStringConverter)
        {
            _argumentToStringConverter = argumentToStringConverter ?? throw new ArgumentNullException(nameof(argumentToStringConverter));
            _separator = new string(argumentsSeparator, 1);
        }

        public string Convert(IEnumerable<CommandLineArgument> source) =>
            string.Join(_separator, source.Select(_argumentToStringConverter.Convert));
    }
}
