namespace Flow.Core
{
    using System;

    internal interface IEnvironment
    {
        OperatingSystem OperatingSystem { get; }

        char CommandLineArgumentsSeparator { get; }

        char CommandLineArgumentsQuote { get; }

        bool IsUnderTeamCity { get; }
    }
}
