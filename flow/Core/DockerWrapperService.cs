namespace Flow.Core
{
    using System;
    using IoC;
    using static Tags;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class DockerWrapperService: IDockerWrapperService
    {
        [NotNull] private readonly IProcessChain _processChain;
        [NotNull] private readonly Func<IInitializableProcessWrapper<DockerWrapperInfo>> _processWrapperFactory;

        public DockerWrapperService(
            [NotNull] IProcessChain processChain,
            [NotNull, Tag(Docker)] Func<IInitializableProcessWrapper<DockerWrapperInfo>> processWrapperFactory)
        {
            _processChain = processChain ?? throw new ArgumentNullException(nameof(processChain));
            _processWrapperFactory = processWrapperFactory ?? throw new ArgumentNullException(nameof(processWrapperFactory));
        }

        public IDisposable Using(DockerWrapperInfo info)
        {
            var wrapper = _processWrapperFactory();
            wrapper.Initialize(info);
            return _processChain.Append(wrapper);
        }
    }
}