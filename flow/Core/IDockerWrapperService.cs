namespace Flow.Core
{
    using System;

    [Public]
    internal interface IDockerWrapperService
    {
        IDisposable Using(DockerWrapperInfo info);
    }
}
