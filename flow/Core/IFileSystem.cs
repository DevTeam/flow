namespace Flow.Core
{
    using System.Collections.Generic;

    internal interface IFileSystem
    {
        void WriteLines(Path filePath, IEnumerable<string> lines);

        IEnumerable<string> ReadLines(Path filePath);
    }
}
