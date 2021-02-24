namespace Flow.Core
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class BasePathNormalizer : IPathNormalizer
    {
        public Path Normalize(Path path) =>
            path.Value
                .Replace(System.IO.Path.GetPathRoot(path.Value), "/")
                .Replace("\\", "/")
                .ToLowerInvariant();
    }
}