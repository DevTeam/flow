namespace Flow.Core
{
    using System;
    using IoC;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class ArgumentToStringConverter : IConverter<CommandLineArgument, string>
    {
        private readonly string _quote;
        private readonly string _separator;

        public ArgumentToStringConverter([NotNull] IEnvironment environment)
        {
            if (environment == null) throw new ArgumentNullException(nameof(environment));
            _quote = new string(environment.CommandLineArgumentsQuote, 1);
            _separator = new string(environment.CommandLineArgumentsSeparator, 1);
        }

        public string Convert(CommandLineArgument source)
        {
            var arg = source.Value;
            if (arg.Contains(_quote))
            {
                string result;
                using (var enumerator = source.Value.GetEnumerator())
                {
                    result = enumerator.Parse();
                }

                if (result != source.Value)
                {
                    arg = result;
                }
            }

            if (arg.Contains(_separator))
            {
                return $"{_quote}{arg}{_quote}";
            }

            return arg;
        }
    }
}
