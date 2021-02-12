namespace Flow.Core
{
    using System;
    using static Tags;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class DockerProcessWrapper: IInitializableProcessWrapper<DockerWrapperInfo>
    {
        private readonly Path _tempPath;
        private bool _initialized;
        private DockerWrapperInfo _wrapperInfo;

        public DockerProcessWrapper(
            [Tag(TempDirectory)] Path tempPath)
        {
            _tempPath = tempPath;
        }

        public void Initialize(DockerWrapperInfo wrapperInfo)
        {
            _wrapperInfo = wrapperInfo;
            _initialized = true;
        }

        public ProcessInfo Wrap(ProcessInfo processInfo)
        {
            if (!_initialized) throw new InvalidOperationException("Not initialized");

            return processInfo;
        }
    }
}
