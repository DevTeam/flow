namespace Flow.Core
{
    internal interface ILocator
    {
        bool TryFind(Path path, out Path fullPath);
    }
}
