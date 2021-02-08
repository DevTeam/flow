namespace Flow.Core
{
    using IoC;

    internal interface IStdErr
    {
        void WriteLine([NotNull] string error);
    }
}
