namespace Flow.Core
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using IoC;
    using static Tags;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class ProcessFactory : IProcessFactory
    {
        [NotNull] private readonly Func<Process, IProcess> _processFactory;
        private readonly Path _workingDirectory;
        [NotNull] private readonly IConverter<CommandLineArgument, string> _argumentToStringConverter;
        [NotNull] private readonly string _separator;

        public ProcessFactory(
            [NotNull] Func<Process, IProcess> processFactory,
            [Tag(WorkingDirectory)] Path workingDirectory,
            [NotNull] IEnvironment environment,
            [NotNull] IConverter<CommandLineArgument, string> argumentToStringConverter)
        {
            if (environment == null) throw new ArgumentNullException(nameof(environment));
            _processFactory = processFactory ?? throw new ArgumentNullException(nameof(processFactory));
            _workingDirectory = workingDirectory;
            _argumentToStringConverter = argumentToStringConverter ?? throw new ArgumentNullException(nameof(argumentToStringConverter));
            _separator = new string(environment.CommandLineArgumentsSeparator, 1);
        }

        public IProcess Create(ProcessInfo info)
        {
            var startInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                FileName = info.Executable.Value,
                WorkingDirectory = (!info.WorkingDirectory.IsEmpty ? info.WorkingDirectory : _workingDirectory).Value,
                Arguments = string.Join(_separator, info.Arguments.Select(_argumentToStringConverter.Convert))
            };

            foreach (var variable in info.Variables)
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