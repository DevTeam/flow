namespace Flow.Core
{
    using System;

    [Public]
    internal interface IWslWrapperService
    {
        IDisposable Using();
    }
}
