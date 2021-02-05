namespace Flow.Services
{
    using System;
    using IoC;

    internal interface IProcess: IDisposable
    {
        ExitCode Run([NotNull] IProcessListener listener);
    }
}