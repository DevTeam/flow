namespace Flow.Core
{
    [Public]
    internal interface ICommandLineService
    {
        ExitCode Execute(ProcessInfo processInfo);
    }
}