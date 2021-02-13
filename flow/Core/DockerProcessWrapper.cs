namespace Flow.Core
{
    using System;
    using IoC;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class DockerProcessWrapper: IInitializableProcessWrapper<DockerWrapperInfo>
    {
        [NotNull] private readonly IDockerArgumentsProvider _dockerArgumentsProvider;
        private bool _initialized;
        private DockerWrapperInfo _wrapperInfo;

        public DockerProcessWrapper(
            [NotNull] IDockerArgumentsProvider dockerArgumentsProvider) =>
            _dockerArgumentsProvider = dockerArgumentsProvider;

        public void Initialize(DockerWrapperInfo wrapperInfo)
        {
            _wrapperInfo = wrapperInfo;
            _initialized = true;
        }

        public ProcessInfo Wrap(ProcessInfo processInfo)
        {
            if (!_initialized) throw new InvalidOperationException("Not initialized");
            return new ProcessInfo(
                "docker",
                processInfo.WorkingDirectory,
                _dockerArgumentsProvider.GetArguments(_wrapperInfo, processInfo),
                processInfo.Variables);
        }
    }
}