namespace Flow.Core
{
    using System;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class InternalEnvironment : IEnvironment
    {
        public const char CommandLineArgumentsSeparatorChar = ' ';
        public const char CommandLineArgumentsQuoteChar = '"';

        public Flow.OperatingSystem OperatingSystem
        {
            get
            {
                switch (Environment.OSVersion.Platform)
                {
                    case PlatformID.Win32S:
                    case PlatformID.Win32Windows:
                    case PlatformID.Win32NT:
                        return Flow.OperatingSystem.Windows;

                    case PlatformID.Unix:
                        return Flow.OperatingSystem.Unix;

                    case PlatformID.MacOSX:
                        return Flow.OperatingSystem.Mac;

                    case PlatformID.WinCE:
                    case PlatformID.Xbox:
                        throw new NotSupportedException();

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public char CommandLineArgumentsSeparator => CommandLineArgumentsSeparatorChar;

        public char CommandLineArgumentsQuote => CommandLineArgumentsQuoteChar;
        
        public bool IsUnderTeamCity =>
            Environment.GetEnvironmentVariable("TEAMCITY_PROJECT_NAME") != null
            || Environment.GetEnvironmentVariable("TEAMCITY_VERSION") != null;
    }
}