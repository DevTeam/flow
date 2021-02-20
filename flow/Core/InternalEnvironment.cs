namespace Flow.Core
{
    using System;
    using System.IO;
    using OperatingSystem = OperatingSystem;
    using Path = Path;

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

        public Path DotnetExecutable
        {
            get
            {
                string dotnetExecutable;
                switch (OperatingSystem)
                {
                    case OperatingSystem.Windows:
                        dotnetExecutable = "dotnet.exe";
                        break;
                    case OperatingSystem.Unix:
                    case OperatingSystem.Mac:
                    case OperatingSystem.Wsl:
                        dotnetExecutable = "dotnet";
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                var dotnetRoot = Environment.GetEnvironmentVariable("DOTNET_ROOT");
                if (dotnetRoot != null)
                {
                    return System.IO.Path.Combine(dotnetRoot, dotnetExecutable);
                }

                var paths = (Environment.GetEnvironmentVariable("PATH") ?? "").Split(';');
                foreach (var path in paths)
                {
                    var dotnetPath = System.IO.Path.Combine(path, dotnetExecutable);
                    if (File.Exists(dotnetPath))
                    {
                        return new Path(dotnetPath);
                    }
                }

                return new Path(dotnetExecutable);
            }
        }
    }
}