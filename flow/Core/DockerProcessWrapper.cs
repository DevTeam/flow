namespace Flow.Core
{
    using IoC;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class DockerProcessWrapper: IProcessWrapper
    {
        [NotNull] private readonly IDockerArgumentsProvider _dockerArgumentsProvider;
        private readonly DockerWrapperInfo _wrapperInfo;

        public DockerProcessWrapper(
            [NotNull] IDockerArgumentsProvider dockerArgumentsProvider,
            DockerWrapperInfo wrapperInfo)
        {
            _dockerArgumentsProvider = dockerArgumentsProvider;
            _wrapperInfo = wrapperInfo;
        }

        public ProcessInfo Wrap(ProcessInfo processInfo) =>
            new ProcessInfo(
                "docker",
                processInfo.WorkingDirectory,
                _dockerArgumentsProvider.GetArguments(_wrapperInfo, processInfo),
                processInfo.Variables);
    }
}