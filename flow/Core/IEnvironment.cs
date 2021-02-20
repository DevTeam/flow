namespace Flow.Core
{
    internal interface IEnvironment
    {
        OperatingSystem OperatingSystem { get; }

        char CommandLineArgumentsSeparator { get; }

        char CommandLineArgumentsQuote { get; }

        bool IsUnderTeamCity { get; }

        Path DotnetExecutable { get; }
    }
}
