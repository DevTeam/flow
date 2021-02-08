namespace Flow.Core
{
    internal interface IProcessWrapper
    {
        void Initialize<TState>(TState state);

        ProcessInfo Wrap(ProcessInfo processInfo);
    }
}
