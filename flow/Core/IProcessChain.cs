namespace Flow.Core
{
    using System;
    using IoC;

    internal interface IProcessChain
    {
        [NotNull] IDisposable Append([NotNull] IProcessWrapper wrapper);
    }
}
