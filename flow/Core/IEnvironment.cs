namespace Flow.Core
{
    internal interface IEnvironment
    {
        char CommandLineArgumentsSeparator { get; }

        char CommandLineArgumentsQuote { get; }
    }
}
