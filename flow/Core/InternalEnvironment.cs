namespace Flow.Core
{
    using System;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class InternalEnvironment : IEnvironment
    {
        public const char CommandLineArgumentsSeparatorChar = ' ';
        public const char CommandLineArgumentsQuoteChar = '"';

        public char CommandLineArgumentsSeparator => CommandLineArgumentsSeparatorChar;

        public char CommandLineArgumentsQuote => CommandLineArgumentsQuoteChar;
    }
}