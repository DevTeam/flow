namespace Flow.Core
{
    using OperatingSystem = OperatingSystem;

    internal interface IPathNormalizer
    {
        Path Normalize(Path path, OperatingSystem targetOperatingSystem);
    }
}
