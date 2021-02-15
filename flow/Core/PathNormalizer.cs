namespace Flow.Core
{
    using System;
    using IoC;
    using static Tags;
    using OperatingSystem = OperatingSystem;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class PathNormalizer : IPathNormalizer
    {
        [NotNull] private readonly string _windowsDirectorySeparatorString;
        [NotNull] private readonly string _linuxDirectorySeparatorString;
        [NotNull] private readonly string _wslRootString;
        private readonly OperatingSystem _operatingSystem;

        public PathNormalizer(
            [NotNull, Tag(WindowsDirectorySeparatorString)] string windowsDirectorySeparatorString,
            [NotNull, Tag(LinuxDirectorySeparatorString)] string linuxDirectorySeparatorString,
            [NotNull, Tag(WslRootString)] string wslRootString,
            OperatingSystem operatingSystem)
        {
            _windowsDirectorySeparatorString = windowsDirectorySeparatorString ?? throw new ArgumentNullException(nameof(windowsDirectorySeparatorString));
            _linuxDirectorySeparatorString = linuxDirectorySeparatorString ?? throw new ArgumentNullException(nameof(linuxDirectorySeparatorString));
            _wslRootString = wslRootString ?? throw new ArgumentNullException(nameof(wslRootString));
            _operatingSystem = operatingSystem;
        }

        public Path Normalize(Path path, OperatingSystem targetOperatingSystem)
        {
            if (targetOperatingSystem == _operatingSystem)
            {
                return path;
            }

            switch (targetOperatingSystem)
            {
                case OperatingSystem.Windows:
                    throw new NotSupportedException();

                case OperatingSystem.Unix:
                case OperatingSystem.Mac:
                    return path.Value
                        .Replace(System.IO.Path.GetPathRoot(path.Value), _linuxDirectorySeparatorString)
                        .Replace(_windowsDirectorySeparatorString, _linuxDirectorySeparatorString)
                        .ToLowerInvariant();

                case OperatingSystem.Wsl:
                    return path.Value
                        .Replace(System.IO.Path.GetPathRoot(path.Value), _wslRootString)
                        .Replace(_windowsDirectorySeparatorString, _linuxDirectorySeparatorString);

                default:
                    throw new ArgumentOutOfRangeException(nameof(targetOperatingSystem), targetOperatingSystem, null);
            }

        }
    }
}