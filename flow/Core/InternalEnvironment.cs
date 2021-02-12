namespace Flow.Core
{
    using System;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class InternalEnvironment : IEnvironment
    {
        public const char CommandLineArgumentsSeparatorChar = ' ';
        public const char CommandLineArgumentsQuoteChar = '"';

        public OperatingSystem OperatingSystem => Environment.OSVersion;

        public char CommandLineArgumentsSeparator => CommandLineArgumentsSeparatorChar;

        public char CommandLineArgumentsQuote => CommandLineArgumentsQuoteChar;
    }
}