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
        private readonly OperatingSystem _operatingSystem;

        public PathNormalizer(
            [NotNull, Tag(WindowsDirectorySeparatorString)] string windowsDirectorySeparatorString,
            [NotNull, Tag(LinuxDirectorySeparatorString)] string linuxDirectorySeparatorString,
            OperatingSystem operatingSystem)
        {
            _windowsDirectorySeparatorString = windowsDirectorySeparatorString ?? throw new ArgumentNullException(nameof(windowsDirectorySeparatorString));
            _linuxDirectorySeparatorString = linuxDirectorySeparatorString ?? throw new ArgumentNullException(nameof(linuxDirectorySeparatorString));
            _operatingSystem = operatingSystem;
        }

        public Path Normalize(Path path, OperatingSystem targetOperatingSystem)
        {
            if (targetOperatingSystem == OperatingSystem.Windows) throw new NotSupportedException();

            if (targetOperatingSystem == _operatingSystem)
            {
                return path;
            }

            var root = System.IO.Path.GetPathRoot(path.Value);
            return path.Value
                .Replace(root, _linuxDirectorySeparatorString)
                .Replace(_windowsDirectorySeparatorString, _linuxDirectorySeparatorString)
                .ToLowerInvariant();
        }
    }
}