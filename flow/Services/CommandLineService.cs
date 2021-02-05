namespace Flow.Services
{
    using System;
    using System.Collections.Generic;
    using IoC;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class CommandLineService : ICommandLineService
    {
        private readonly IProcessFactory _processFactory;
        [NotNull] private readonly IProcessListener _processListener;

        public CommandLineService(
            [NotNull] IProcessFactory processFactory,
            [NotNull, Tag("stdOutErr")] IProcessListener processListener)
        {
            _processFactory = processFactory ?? throw new ArgumentNullException(nameof(processFactory));
            _processListener = processListener ?? throw new ArgumentNullException(nameof(processListener));
        }

        public ExitCode Execute(Path executable, Path workingDirectory, IEnumerable<CommandLineArgument> arguments, IEnumerable<EnvironmentVariable> variables)
        {
            if (executable.Value == null) throw new ArgumentNullException(nameof(executable));
            if (arguments == null) throw new ArgumentNullException(nameof(arguments));
            if (variables == null) throw new ArgumentNullException(nameof(variables));

            using (var process = _processFactory.Create(executable, workingDirectory, arguments, variables))
            {
                return process.Run(_processListener);
            }
        }
    }
}