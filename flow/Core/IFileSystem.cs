namespace Flow.Core
{
    using System.Collections.Generic;
    using IoC;

    internal interface IFileSystem
    {
        void WriteLines(Path filePath, [NotNull] IEnumerable<string> lines, OperatingSystem operatingSystem);
    }
}
