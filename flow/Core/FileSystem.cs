namespace Flow.Core
{
    using System.Collections.Generic;
    using System.IO;
    using Path = Path;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class FileSystem : IFileSystem
    {
        public void WriteLines(Path filePath, IEnumerable<string> lines)
        {
            var dir = System.IO.Path.GetDirectoryName(filePath.Value);
            if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            
            File.WriteAllLines(filePath.Value, lines);
        }

        public IEnumerable<string> ReadLines(Path filePath) =>
            File.ReadLines(filePath.Value);
    }
}