namespace Flow.Core
{
    using System;
    using IoC;

    internal interface IProcess: IDisposable
    {
        ExitCode Run([NotNull] IProcessListener listener);
    }
}