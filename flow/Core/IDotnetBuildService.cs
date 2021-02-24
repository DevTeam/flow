namespace Flow.Core
{
    [Public]
    internal interface IDotnetBuildService
    {
        BuildResult Execute(DotnetBuildInfo info);
    }
}
