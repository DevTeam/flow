namespace Flow.Core
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using IoC;
    using static Tags;

    internal class Locator: ILocator
    {
        [NotNull] private readonly IProcessFactory _processFactory;
        private readonly Path _workingDirectory;

        public Locator(
            [NotNull] IProcessFactory processFactory,
            [Tag(WorkingDirectory)] Path workingDirectory)
        {
            _processFactory = processFactory ?? throw new ArgumentNullException(nameof(processFactory));
            _workingDirectory = workingDirectory;
        }

        internal Path SearchTool { get; set; }

        public bool TryFind(Path path, out Path fullPath)
        {
            var processInfo = new ProcessInfo(SearchTool, _workingDirectory, new[] { new CommandLineArgument(path.Value) }, Enumerable.Empty<EnvironmentVariable>());
            var process = _processFactory.Create(processInfo);
            var processListener = new ProcessListener();
            if (process.Run(processListener).Value == 0 && !string.IsNullOrWhiteSpace(processListener.FirstLine))
            {
                fullPath = new Path(processListener.FirstLine);
                return true;
            }

            fullPath = default(Path);
            return false;
        }

        private class ProcessListener : IProcessListener
        {
            private bool _hasLine;

            [NotNull] public string FirstLine { get; private set; } = string.Empty;

            public void OnStart(ProcessStartInfo startInfo) { }

            public void OnStdOut(string line)
            {
                if (_hasLine)
                {
                    return;
                }

                FirstLine = line;
                _hasLine = true;
            }

            public void OnStdErr(string line) { }

            public void OnExitCode(ExitCode exitCode)
            {
                if (exitCode.Value != 0)
                {
                    FirstLine = string.Empty;
                }
            }
        }
    }
}
