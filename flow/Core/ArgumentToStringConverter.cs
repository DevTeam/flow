namespace Flow.Core
{
    using static Tags;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class ArgumentToStringConverter : IConverter<CommandLineArgument, string>
    {
        private readonly string _quote;
        private readonly string _separator;

        public ArgumentToStringConverter(
            [Tag(ArgumentsSeparatorChar)] char argumentsSeparator,
            [Tag(ArgumentsQuoteChar)] char argumentsQuote)
        {
            _quote = new string(argumentsQuote, 1);
            _separator = new string(argumentsSeparator, 1);
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
