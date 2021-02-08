namespace Flow.Core
{
    using System;
    using System.Collections.Generic;
    using IoC;

    internal readonly struct ProcessInfo
    {
        public readonly Path Executable;
        public readonly Path WorkingDirectory;
        [NotNull] public readonly IEnumerable<CommandLineArgument> Arguments;
        [NotNull] public readonly IEnumerable<EnvironmentVariable> Variables;

        public ProcessInfo(Path executable, Path workingDirectory, [NotNull] IEnumerable<CommandLineArgument> arguments, [NotNull] IEnumerable<EnvironmentVariable> variables)
        {
            Executable = executable;
            WorkingDirectory = workingDirectory;
            Arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
            Variables = variables ?? throw new ArgumentNullException(nameof(variables));
        }
    }
}
