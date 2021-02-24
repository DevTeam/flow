namespace Flow.Core
{
    internal interface IToolResolver
    {
        bool TryResolve(Tool tool, out Path path);
    }
}
