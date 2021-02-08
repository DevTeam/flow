namespace Flow.Core
{
    using IoC;

    internal interface IProcessFactory
    {
        [NotNull] IProcess Create(ProcessInfo info);
    }
}