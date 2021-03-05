namespace Flow.Core
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using IoC;
    using static Tags;

    internal class Locator: ILocator, IProcessListener
    {
        [NotNull] private readonly IProcessFactory _processFactory;
        private readonly Path _workingDirectory;
        private string _firstLine;

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
            if (process.Run(this).Value != 0)
            {
                throw new InvalidOperationException($"Cannot run {SearchTool}.");
            }

            if (!string.IsNullOrWhiteSpace(_firstLine))
            {
                fullPath = new Path(_firstLine);
                return true;
            }

            fullPath = default(Path);
            return false;
        }

        void IProcessListener.OnStart(ProcessStartInfo startInfo) { }

        void IProcessListener.OnStdOut(string line)
        {
            if (_firstLine == null)
            {
                _firstLine = line;
            }
        }

        void IProcessListener.OnStdErr(string line) { }

        void IProcessListener.OnExitCode(ExitCode exitCode) { }
    }
}
