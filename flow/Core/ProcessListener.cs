namespace Flow.Core
{
    using System;
    using System.Diagnostics;
    using IoC;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class ProcessListener: IProcessListener
    {
        [NotNull] private readonly ILog<ProcessListener> _log;
        private readonly IColorfulStdOut _stdOut;
        private readonly IStdErr _stdErr;

        public ProcessListener(
            [NotNull] ILog<ProcessListener> log,
            [NotNull] IColorfulStdOut stdOut,
            [NotNull] IStdErr stdErr)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _stdOut = stdOut ?? throw new ArgumentNullException(nameof(stdOut));
            _stdErr = stdErr ?? throw new ArgumentNullException(nameof(stdErr));
        }

        public void OnStart(ProcessStartInfo startInfo)
        {
            _log.Info(() => new Text("Starting: ", Color.Header) + new Text($"{System.IO.Path.GetFullPath(startInfo.FileName)} {startInfo.Arguments}"));
            _log.Info(() => new Text("in directory: ", Color.Header) + new Text($"{System.IO.Path.GetFullPath(startInfo.WorkingDirectory)}"));

        }

        public void OnStdOut(string line)
        {
            if (line == null) throw new ArgumentNullException(nameof(line));
            _stdOut.WriteLine(new Text(line));
        }

        public void OnStdErr(string line)
        {
            if (line == null) throw new ArgumentNullException(nameof(line));
            _stdErr.WriteLine(line);
        }

        public void OnExitCode(ExitCode exitCode) =>
            _log.Info(() => new Text("Exit code: ", Color.Header) + new Text(exitCode.ToString()));
    }
}
