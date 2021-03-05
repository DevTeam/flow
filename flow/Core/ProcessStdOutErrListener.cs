namespace Flow.Core
{
    using System;
    using System.Diagnostics;
    using IoC;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class ProcessStdOutErrListener: IProcessListener
    {
        private readonly IColorfulStdOut _stdOut;
        private readonly IStdErr _stdErr;

        public ProcessStdOutErrListener(
            [NotNull] IColorfulStdOut stdOut,
            [NotNull] IStdErr stdErr)
        {
            _stdOut = stdOut ?? throw new ArgumentNullException(nameof(stdOut));
            _stdErr = stdErr ?? throw new ArgumentNullException(nameof(stdErr));
        }

        public void OnStart(ProcessStartInfo startInfo)
        {
            _stdOut.WriteLine(new Text("Starting: ", Color.Header), new Text($"{System.IO.Path.GetFullPath(startInfo.FileName)} {startInfo.Arguments}"));
            _stdOut.WriteLine(new Text("in directory: ", Color.Header), new Text($"{System.IO.Path.GetFullPath(startInfo.WorkingDirectory)}"));
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
            _stdOut.WriteLine(new Text("Exit code: ", Color.Header), new Text(exitCode.ToString()));
    }
}
