namespace Flow.Core
{
    using System;
    using static Tags;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class DockerProcessWrapper: IProcessWrapper
    {
        private readonly Path _tempPath;
        private bool _initialized;
        private DockerWrapperInfo _state;

        public DockerProcessWrapper(
            [Tag(TempDirectory)] Path tempPath)
        {
            _tempPath = tempPath;
        }

        public void Initialize<TState>(TState state)
        {
            _state = (DockerWrapperInfo) (object) state;
            _initialized = true;
        }

        public ProcessInfo Wrap(ProcessInfo processInfo)
        {
            if (!_initialized) throw new InvalidOperationException("Not initialized");

            return processInfo;
        }
    }
}
