namespace Flow.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using IoC;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class BuildListener: IProcessListener<BuildResult>, IBuildVisitor
    {
        [NotNull] private readonly ILog<BuildListener> _log;
        private readonly IColorfulStdOut _stdOut;
        [NotNull] private readonly IStdErr _stdErr;
        [NotNull] private readonly IMessageProcessor _messageProcessor;
        [NotNull] private readonly List<BuildError> _errors = new List<BuildError>();
        [NotNull] private readonly List<BuildWarning> _warnings = new List<BuildWarning>();
        private ExitCode? _exitCode;

        public BuildListener(
            [NotNull] ILog<BuildListener> log,
            [NotNull] IColorfulStdOut stdOut,
            [NotNull] IStdErr stdErr,
            [NotNull] IMessageProcessor messageProcessor)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _stdOut = stdOut ?? throw new ArgumentNullException(nameof(stdOut));
            _stdErr = stdErr ?? throw new ArgumentNullException(nameof(stdErr));
            _messageProcessor = messageProcessor ?? throw new ArgumentNullException(nameof(messageProcessor));
        }

        public void OnStart(ProcessStartInfo startInfo)
        {
            _log.Info(() => new Text($"Starting: {System.IO.Path.GetFileName(startInfo.FileName)} {startInfo.Arguments}", Color.Header));
            _log.Info(() => new Text($"in directory: {System.IO.Path.GetFullPath(startInfo.WorkingDirectory)}", Color.Header));
        }

        public void OnStdOut(string line)
        {
            if (line == null) throw new ArgumentNullException(nameof(line));
            if (!_messageProcessor.ProcessServiceMessages(line, this))
            {
                _stdOut.WriteLine(new Text(line));
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
            _log.Info(() => new Text("Exit code: ", Color.Header) + new Text(exitCode.ToString(), exitCode.Value == 0 ? Color.Success : Color.Error));
        }

        public BuildResult Result => 
            new BuildResult(
                _exitCode.HasValue && _exitCode?.Value == 0 && !_errors.Any(),
                _errors.AsReadOnly(),
                _warnings.AsReadOnly());

        public void Visit(BuildError error) =>
            _errors.Add(error);

        public void Visit(BuildWarning warning) =>
            _warnings.Add(warning);
    }
}
