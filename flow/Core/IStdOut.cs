namespace Flow.Core
{
    using IoC;

    internal interface IStdOut
    {
        void WriteLine([NotNull] string text);
    }
}
