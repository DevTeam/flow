namespace Flow.Services
{
    using System.Collections.Generic;
    using IoC;

    [Public]
    internal interface ICommandLineService
    {
        ExitCode Execute(Path executable, Path workingDirectory, [NotNull] IEnumerable<CommandLineArgument> arguments, [NotNull] IEnumerable<EnvironmentVariable> variables);
    }
}