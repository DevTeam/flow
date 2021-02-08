namespace Flow.Core
{
    using System.Collections.Generic;
    using IoC;

    internal interface IProcessFactory
    {
        [NotNull] IProcess Create(Path executable, Path workingDirectory, [NotNull] IEnumerable<CommandLineArgument> arguments, [NotNull] IEnumerable<EnvironmentVariable> variables);
    }
}