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
        internal const string FlowVersionEnvName = "FLOW_VERSION";
        [NotNull] private readonly Func<Process, IProcess> _processFactory;
        private readonly Path _workingDirectory;
        private readonly string _flowVersion;
        [NotNull] private readonly IConverter<CommandLineArgument, string> _argumentToStringConverter;
        [NotNull] private readonly string _separator;

        public ProcessFactory(
            [NotNull] Func<Process, IProcess> processFactory,
            [Tag(WorkingDirectory)] Path workingDirectory,
            [Tag(FlowVersionString)] [NotNull] string flowVersion,
            [Tag(ArgumentsSeparatorChar)] char separator,
            [NotNull] IConverter<CommandLineArgument, string> argumentToStringConverter)
        {
            _processFactory = processFactory ?? throw new ArgumentNullException(nameof(processFactory));
            _workingDirectory = workingDirectory;
            _flowVersion = flowVersion ?? throw new ArgumentNullException(nameof(flowVersion));
            _argumentToStringConverter = argumentToStringConverter ?? throw new ArgumentNullException(nameof(argumentToStringConverter));
            _separator = new string(separator, 1);
        }

        public IProcess Create(ProcessInfo info)
        {
            var workingDirectory = !info.WorkingDirectory.IsEmpty ? info.WorkingDirectory : _workingDirectory;
            var startInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                FileName = info.Executable.Value,
                WorkingDirectory = workingDirectory.Value,
                Arguments = string.Join(_separator, info.Arguments.Select(_argumentToStringConverter.Convert))
            };

            startInfo.Environment[FlowVersionEnvName] = _flowVersion;
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