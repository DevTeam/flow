namespace Flow.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using IoC;
    using static Tags;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class DotnetBuildService : IDotnetBuildService
    {
        private readonly Path _dotnetExecutable;
        private readonly Path _workingDirectory;
        [NotNull] private readonly IProcessFactory _processFactory;
        [NotNull] private readonly IProcessListener _processListener;

        public DotnetBuildService(
            [Tag(DotnetExecutable)] Path dotnetExecutable,
            [Tag(WorkingDirectory)] Path workingDirectory,
            [NotNull] IProcessFactory processFactory,
            [NotNull, Tag(StdOutErr)] IProcessListener processListener)
        {
            _dotnetExecutable = dotnetExecutable;
            _workingDirectory = workingDirectory;
            _processFactory = processFactory ?? throw new ArgumentNullException(nameof(processFactory));
            _processListener = processListener ?? throw new ArgumentNullException(nameof(processListener));
        }

        public BuildResult Execute()
        {
            var process = _processFactory.Create(
                new ProcessInfo(
                    _dotnetExecutable,
                    _workingDirectory,
                    GetArgs(),
                    Enumerable.Empty<EnvironmentVariable>()));

            var exitCode = process.Run(_processListener);
            return new BuildResult(exitCode.Value == 0);
        }

        private IEnumerable<CommandLineArgument> GetArgs()
        {
            yield return "build";
        }
    }
}