namespace Flow.Services
{
    using IoC;

    internal interface IStdErr
    {
        void WriteLine([NotNull] string error);
    }
}
