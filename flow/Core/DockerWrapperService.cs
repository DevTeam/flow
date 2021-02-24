namespace Flow.Core
{
    using System;
    using IoC;
    using static Tags;
    using OperatingSystem = OperatingSystem;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class DockerWrapperService: IDockerWrapperService
    {
        [NotNull] private readonly IChain<IProcessWrapper> _processChain;
        [NotNull] private readonly IChain<OperatingSystem> _osChain;
        [NotNull] private readonly Func<DockerWrapperInfo, IProcessWrapper> _dockerProcessWrapperFactory;
        [NotNull] private readonly IProcessWrapper _cmdProcessWrapper;
        [NotNull] private readonly IProcessWrapper _shProcessWrapper;

        public DockerWrapperService(
            [NotNull] IChain<IProcessWrapper> processChain,
            [NotNull] IChain<OperatingSystem> osChain,
            [NotNull, Tag(DockerWrapper)] Func<DockerWrapperInfo, IProcessWrapper> dockerProcessWrapperFactory,
            [NotNull, Tag(CmdScriptWrapper)] IProcessWrapper cmdProcessWrapper,
            [NotNull, Tag(ShScriptWrapper)] IProcessWrapper shProcessWrapper)
        {
            _processChain = processChain ?? throw new ArgumentNullException(nameof(processChain));
            _osChain = osChain ?? throw new ArgumentNullException(nameof(osChain));
            _dockerProcessWrapperFactory = dockerProcessWrapperFactory ?? throw new ArgumentNullException(nameof(dockerProcessWrapperFactory));
            _cmdProcessWrapper = cmdProcessWrapper ?? throw new ArgumentNullException(nameof(cmdProcessWrapper));
            _shProcessWrapper = shProcessWrapper ?? throw new ArgumentNullException(nameof(shProcessWrapper));
        }

        public IDisposable Using(DockerWrapperInfo info) =>
            Disposable.Create(
                _osChain.Append(info.Platform),
                _processChain.Append(_dockerProcessWrapperFactory(info)),
                _processChain.Append(info.Platform == OperatingSystem.Windows ? _cmdProcessWrapper : _shProcessWrapper));
    }
}