namespace Flow.Core
{
    using System;
    using System.Linq;
    using IoC;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class DotnetWrapperService : IDotnetWrapperService, IProcessWrapper
    {
        [NotNull] private readonly IProcessChain _processChain;
        [NotNull] private readonly IFileSystem _fileSystem;
        private DotnetWrapperInfo _info;

        public DotnetWrapperService(
            [NotNull] IProcessChain processChain,
            [NotNull] IFileSystem fileSystem)
        {
            _processChain = processChain ?? throw new ArgumentNullException(nameof(processChain));
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        public IDisposable Using(DotnetWrapperInfo info)
        {
            _info = info;
            return Disposable.Create(_processChain.Append(this));
        }

        public ProcessInfo Wrap(ProcessInfo processInfo)
        {
            var workingDirectory = processInfo.WorkingDirectory;
            if (!_info.SolutionDirectory.IsEmpty)
            {
                if (_fileSystem.IsPathRooted(_info.SolutionDirectory))
                {
                    workingDirectory = _info.SolutionDirectory;
                }
                else
                {
                    workingDirectory = workingDirectory + _info.SolutionDirectory;
                }
            }

            return new ProcessInfo(
                !processInfo.Executable.IsEmpty ? processInfo.Executable : _info.DotnetExecutable,
                workingDirectory,
                processInfo.Arguments,
                processInfo.Variables.Concat(_info.EnvironmentVariables));
        }
    }
}