namespace Flow.Services
{
    internal interface IEnvironment
    {
        char CommandLineArgumentsSeparator { get; }

        char CommandLineArgumentsQuote { get; }

        Path WorkingDirectory { get; }
    }
}
