namespace Flow.Services
{
    using IoC;

    internal interface IStdOut
    {
        void WriteLine([NotNull] string text);
    }
}
