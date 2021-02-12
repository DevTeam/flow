namespace Flow.Core
{
    using System;
    using IoC;
    using static Tags;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class DockerProcessWrapper: IInitializableProcessWrapper<DockerWrapperInfo>
    {
        private readonly Path _tempPath;
        [NotNull] private readonly IEnvironment _environment;
        [NotNull] private readonly IProcessWrapper _processWrapper;
        private bool _initialized;
        private DockerWrapperInfo _wrapperInfo;

        public DockerProcessWrapper(
            [Tag(TempDirectory)] Path tempPath,
            [NotNull] IEnvironment environment,
            [NotNull, Tag(Script)] IProcessWrapper processWrapper)
        {
            _tempPath = tempPath;
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _processWrapper = processWrapper;
        }

        public void Initialize(DockerWrapperInfo wrapperInfo)
        {
            _wrapperInfo = wrapperInfo;
            _initialized = true;
        }

        public ProcessInfo Wrap(ProcessInfo processInfo)
        {
            if (!_initialized) throw new InvalidOperationException("Not initialized");

            var scriptProcessInfo = _processWrapper.Wrap(processInfo);
            return processInfo;
        }
    }
}
