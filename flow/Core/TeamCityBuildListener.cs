namespace Flow.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using IoC;
    using JetBrains.TeamCity.ServiceMessages.Read;
    using static Tags;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class TeamCityBuildListener: IProcessListener<BuildResult>, IBuildVisitor, IServiceMessageProcessor
    {
        [NotNull] private readonly IStdOut _stdOut;
        [NotNull] private readonly IStdErr _stdErr;
        [NotNull] private readonly IServiceMessageParser _serviceMessageParser;
        [NotNull] private readonly Func<IBuildLogFlow> _flowFactory;
        [NotNull] private readonly Dictionary<string, IBuildLogFlow> _flows = new Dictionary<string, IBuildLogFlow>();
        [NotNull] private readonly List<BuildError> _errors = new List<BuildError>();
        [NotNull] private readonly List<BuildWarning> _warnings = new List<BuildWarning>();
        private ExitCode? _exitCode;

        public TeamCityBuildListener(
            [NotNull, Tag(Base)] IStdOut stdOut,
            [NotNull] IStdErr stdErr,
            [NotNull] IServiceMessageParser serviceMessageParser,
            [NotNull] Func<IBuildLogFlow> flowFactory)
        {
            _stdOut = stdOut ?? throw new ArgumentNullException(nameof(stdOut));
            _stdErr = stdErr ?? throw new ArgumentNullException(nameof(stdErr));
            _serviceMessageParser = serviceMessageParser ?? throw new ArgumentNullException(nameof(serviceMessageParser));
            _flowFactory = flowFactory ?? throw new ArgumentNullException(nameof(flowFactory));
        }

        public void OnStart(ProcessStartInfo startInfo)
        {
            _stdOut.WriteLine($"Starting: {System.IO.Path.GetFullPath(startInfo.FileName)} {startInfo.Arguments}");
            _stdOut.WriteLine($"in directory: {System.IO.Path.GetFullPath(startInfo.WorkingDirectory)}");
        }

        public void OnStdOut(string line)
        {
            if (line == null) throw new ArgumentNullException(nameof(line));
            if (!ProcessServiceMessages(line))
            {
                _stdOut.WriteLine(line);
            }
        }

        public void OnStdErr(string line)
        {
            if (line == null) throw new ArgumentNullException(nameof(line));
            _stdErr.WriteLine(line);
        }

        public void OnExitCode(ExitCode exitCode)
        {
            _exitCode = exitCode;
            _stdOut.WriteLine($"Exit code: {exitCode}");
        }

        public bool ProcessServiceMessages(string text)
        {
            if (text == null)
            {
                return false;
            }

            var processed = false;
            foreach (var message in _serviceMessageParser.ParseServiceMessages(text))
            {
                var flowId = message.GetValue("parent") ?? message.GetValue("flowId") ?? string.Empty;
                if (!_flows.TryGetValue(flowId, out var flow))
                {
                    flow = _flowFactory();
                    _flows.Add(flowId, flow);
                }

                switch (message.Name?.ToLowerInvariant())
                {
                    case "flowstarted":
                        _flows[message.GetValue("flowId")] = flow.CreateChild();
                        processed = true;
                        break;

                    case "flowfinished":
                        _flows.Remove(flowId);
                        processed = true;
                        break;

                    default:
                        processed |= flow.ProcessMessage(this, this, message);
                        break;
                }
            }

            return processed;
        }

        public BuildResult Result => 
            new BuildResult(
                _exitCode.HasValue && _exitCode?.Value == 0,
                _errors.AsReadOnly(),
                _warnings.AsReadOnly());

        void IBuildVisitor.Visit(BuildError error) =>
            _errors.Add(error);

        void IBuildVisitor.Visit(BuildWarning warning) =>
            _warnings.Add(warning);
    }
}
