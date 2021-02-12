namespace Flow.Core
{
    using System;
    using IoC;
    using static Tags;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class DockerWrapperService: IDockerWrapperService
    {
        [NotNull] private readonly IProcessChain _processChain;
        [NotNull] private readonly Func<IInitializableProcessWrapper<DockerWrapperInfo>> _dockerProcessWrapperFactory;
        [NotNull] private readonly IProcessWrapper _scriptProcessWrapper;

        public DockerWrapperService(
            [NotNull] IProcessChain processChain,
            [NotNull, Tag(Docker)] Func<IInitializableProcessWrapper<DockerWrapperInfo>> dockerProcessWrapperFactory,
            [NotNull, Tag(Script)] IProcessWrapper scriptProcessWrapper)
        {
            _processChain = processChain ?? throw new ArgumentNullException(nameof(processChain));
            _dockerProcessWrapperFactory = dockerProcessWrapperFactory ?? throw new ArgumentNullException(nameof(dockerProcessWrapperFactory));
            _scriptProcessWrapper = scriptProcessWrapper ?? throw new ArgumentNullException(nameof(scriptProcessWrapper));
        }

        public IDisposable Using(DockerWrapperInfo info)
        {
            var dockerProcessWrapper = _dockerProcessWrapperFactory();
            dockerProcessWrapper.Initialize(info);

            return Disposable.Create(
                _processChain.Append(dockerProcessWrapper),
                _processChain.Append(_scriptProcessWrapper));
        }
    }
}