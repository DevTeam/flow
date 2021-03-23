namespace Flow.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using OperatingSystem = OperatingSystem;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class DefaultEnvironment : IEnvironment, IPathNormalizer
    {
        public OperatingSystem OperatingSystem
        {
            get
            {
                switch (Environment.OSVersion.Platform)
                {
                    case PlatformID.Win32S:
                    case PlatformID.Win32Windows:
                    case PlatformID.Win32NT:
                        return OperatingSystem.Windows;

                    case PlatformID.Unix:
                        return OperatingSystem.Unix;

                    case PlatformID.MacOSX:
                        return OperatingSystem.Mac;

                    case PlatformID.WinCE:
                    case PlatformID.Xbox:
                        throw new NotSupportedException();

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public IPathNormalizer PathNormalizer => this;

        public IEnumerable<EnvironmentVariable> Variables => 
            Environment.GetEnvironmentVariables()
                .Cast<DictionaryEntry>()
                .Select(i => Tuple.Create((string)i.Key, (string)i.Value))
                .Where(i => !string.IsNullOrWhiteSpace(i.Item1))
                .Select(i => new EnvironmentVariable(i.Item1, i.Item2));

        public Path Normalize(Path path) =>
            path.Value
                .Replace(System.IO.Path.GetPathRoot(path.Value), "/")
                .Replace("\\", "/")
                .ToLowerInvariant();
    }
}