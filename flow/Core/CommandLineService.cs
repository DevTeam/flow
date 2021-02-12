namespace Flow.Core
{
    using System;
    using IoC;
    using static Tags;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class CommandLineService : ICommandLineService
    {
        private readonly IProcessFactory _processFactory;
        [NotNull] private readonly IProcessListener _processListener;

        public CommandLineService(
            [NotNull] IProcessFactory processFactory,
            [NotNull, Tag(StdOutErr)] IProcessListener processListener)
        {
            _processFactory = processFactory ?? throw new ArgumentNullException(nameof(processFactory));
            _processListener = processListener ?? throw new ArgumentNullException(nameof(processListener));
        }

        public ExitCode Execute(ProcessInfo processInfo)
        {
            using (var process = _processFactory.Create(processInfo))
            {
                return process.Run(_processListener);
            }
        }
    }
}