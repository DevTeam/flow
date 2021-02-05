namespace Flow.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using IoC;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class ProcessFactory : IProcessFactory
    {
        [NotNull] private readonly Func<Process, IProcess> _processFactory;
        [NotNull] private readonly IEnvironment _environment;

        public ProcessFactory(
            [NotNull] Func<Process, IProcess> processFactory,
            [NotNull] IEnvironment environment)
        {
            _processFactory = processFactory ?? throw new ArgumentNullException(nameof(processFactory));
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        public IProcess Create(Path executable, Path workingDirectory, IEnumerable<CommandLineArgument> arguments, IEnumerable<EnvironmentVariable> variables)
        {
            if (arguments == null) throw new ArgumentNullException(nameof(arguments));
            if (variables == null) throw new ArgumentNullException(nameof(variables));

            var startInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                FileName = executable.Value,
                WorkingDirectory = (!workingDirectory.IsEmpty ? workingDirectory : _environment.WorkingDirectory).Value,
                Arguments = string.Join(" ", arguments.Select(i => i.Value))
            };

            foreach (var variable in variables)
            {
                startInfo.Environment[variable.Name] = variable.Value;
            }

            var process = new Process
            {
                StartInfo = startInfo,
                EnableRaisingEvents = true
            };
            
            return _processFactory(process);
        }
    }
}