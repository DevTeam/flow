namespace Flow.Core
{
    using System;

    [Public]
    internal interface IDotnetWrapperService
    {
        IDisposable Using(DotnetWrapperInfo info);
    }
}
