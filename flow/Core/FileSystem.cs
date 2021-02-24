namespace Flow.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using IoC;
    using static Tags;
    using Path = Path;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class FileSystem : IFileSystem
    {
        [NotNull] private readonly Func<string> _newLineString;
        
        public FileSystem(
            [NotNull, Tag(NewLineString)] Func<string> newLineString) =>
            _newLineString = newLineString ?? throw new ArgumentNullException(nameof(newLineString));

        public bool IsPathRooted(Path path) => System.IO.Path.IsPathRooted(path.Value);

        public void WriteLines(Path filePath, IEnumerable<string> lines)
        {
            if (lines == null) throw new ArgumentNullException(nameof(lines));
            var dir = System.IO.Path.GetDirectoryName(filePath.Value);
            if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var newLineString = _newLineString();
            using (var file = File.CreateText(filePath.Value))
            {
                foreach (var line in lines)
                {
                    file.Write(line);
                    file.Write(newLineString);
                }
            }
        }
    }
}