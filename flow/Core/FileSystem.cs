namespace Flow.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using IoC;
    using static Tags;
    using OperatingSystem = OperatingSystem;
    using Path = Path;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class FileSystem : IFileSystem
    {
        [NotNull] private readonly string _windowsNewLineString;
        [NotNull] private readonly string _linuxNewLineString;

        public FileSystem(
            [NotNull, Tag(WindowsNewLineString)] string windowsNewLineString,
            [NotNull, Tag(LinuxNewLineString)] string linuxNewLineString)
        {
            _windowsNewLineString = windowsNewLineString ?? throw new ArgumentNullException(nameof(windowsNewLineString));
            _linuxNewLineString = linuxNewLineString ?? throw new ArgumentNullException(nameof(linuxNewLineString));
        }

        public bool IsPathRooted(Path path) => System.IO.Path.IsPathRooted(path.Value);

        public void WriteLines(Path filePath, IEnumerable<string> lines, OperatingSystem operatingSystem)
        {
            if (lines == null) throw new ArgumentNullException(nameof(lines));
            var dir = System.IO.Path.GetDirectoryName(filePath.Value);
            if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var newLineString = GetNewLineString(operatingSystem);
            using (var file = File.CreateText(filePath.Value))
            {
                foreach (var line in lines)
                {
                    file.Write(line);
                    file.Write(newLineString);
                }
            }
        }

        private string GetNewLineString(OperatingSystem operatingSystem) =>
            operatingSystem == OperatingSystem.Windows ? _windowsNewLineString : _linuxNewLineString;
    }
}