namespace Flow.Core
{
    using System;
    using IoC;
    using static Tags;
    using OperatingSystem = OperatingSystem;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class DockerWrapperService: IDockerWrapperService
    {
        private readonly OperatingSystem _operatingSystem;
        [NotNull] private readonly IProcessChain _processChain;
        [NotNull] private readonly Func<IInitializableProcessWrapper<DockerWrapperInfo>> _dockerProcessWrapperFactory;
        [NotNull] private readonly IProcessWrapper _cmdProcessWrapper;
        [NotNull] private readonly IProcessWrapper _shProcessWrapper;

        public DockerWrapperService(
            OperatingSystem operatingSystem,
            [NotNull] IProcessChain processChain,
            [NotNull, Tag(DockerWrapper)] Func<IInitializableProcessWrapper<DockerWrapperInfo>> dockerProcessWrapperFactory,
            [NotNull, Tag(CmdScriptWrapper)] IProcessWrapper cmdProcessWrapper,
            [NotNull, Tag(ShScriptWrapper)] IProcessWrapper shProcessWrapper)
        {
            _operatingSystem = operatingSystem;
            _processChain = processChain ?? throw new ArgumentNullException(nameof(processChain));
            _dockerProcessWrapperFactory = dockerProcessWrapperFactory ?? throw new ArgumentNullException(nameof(dockerProcessWrapperFactory));
            _cmdProcessWrapper = cmdProcessWrapper ?? throw new ArgumentNullException(nameof(cmdProcessWrapper));
            _shProcessWrapper = shProcessWrapper ?? throw new ArgumentNullException(nameof(shProcessWrapper));
        }

        public IDisposable Using(DockerWrapperInfo info)
        {
            var dockerProcessWrapper = _dockerProcessWrapperFactory();
            dockerProcessWrapper.Initialize(info);

            return Disposable.Create(
                _processChain.Append(GetProcessWrapper(_operatingSystem)),
                _processChain.Append(dockerProcessWrapper),
                _processChain.Append(GetProcessWrapper(info.Platform)));
        }

        private IProcessWrapper GetProcessWrapper(OperatingSystem operatingSystem) =>
            operatingSystem == OperatingSystem.Windows ? _cmdProcessWrapper : _shProcessWrapper;
    }
}