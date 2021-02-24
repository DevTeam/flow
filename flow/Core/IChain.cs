namespace Flow.Core
{
    using System;
    using IoC;

    internal interface IChain<T>
    {
        T Current { get; }

        [NotNull] IDisposable Append([NotNull] T link);
    }
}
