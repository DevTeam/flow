namespace Flow.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using IoC;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal class FlowProcess : IProcess
    {
        [NotNull] private readonly ILog<FlowProcess> _log;
        private readonly Process _process;
        
        public FlowProcess(
            [NotNull] ILog<FlowProcess> log,
            [NotNull] Process process)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _process = process ?? throw new ArgumentNullException(nameof(process));
        }

        public ExitCode Run(IProcessListener listener)
        {
            if (listener == null) throw new ArgumentNullException(nameof(listener));

            void OnOutputDataReceived(object sender, DataReceivedEventArgs args) =>
                listener.OnStdOut(args.Data ?? string.Empty);

            void OnErrorDataReceived(object sender, DataReceivedEventArgs args) =>
                listener.OnStdErr(args.Data ?? string.Empty);

            _process.OutputDataReceived += OnOutputDataReceived;
            _process.ErrorDataReceived += OnErrorDataReceived;
            listener.OnStart(_process.StartInfo);

            _log.Trace(() =>
            {
                var text = new List<Text>
                {
                    new Text("Starting", Color.Header),
                    Text.NewLine,
                    "FileName: ",
                    _process.StartInfo.FileName,
                    Text.NewLine,
                    "WorkingDirectory: ",
                    _process.StartInfo.WorkingDirectory,
                    Text.NewLine,
                    "Arguments: ",
                    _process.StartInfo.Arguments,
                    Text.NewLine
                };

                if (_process.StartInfo.Environment.Count > 0)
                {
                    text.Add("Environment Variables:");
                    text.Add(Text.NewLine);
                    foreach (var env in _process.StartInfo.Environment)
                    {
                        text.Add(Text.Tab);
                        text.Add($"{env.Key}={env.Value}");
                        text.Add(Text.NewLine);
                    }
                }

                return text.ToArray();
            });

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            _process.Start();

            _log.Trace(() => new[]
            {
                new Text("Started", Color.Header),
                Text.NewLine,
                new Text("Id: "),
                new Text(_process.Id.ToString(), Color.Header),
                Text.NewLine,
                new Text("Process Name: "),
                new Text(_process.ProcessName),
                Text.NewLine,
                new Text("Base Priority: "),
                new Text(_process.BasePriority.ToString()),
                Text.NewLine,
                new Text("Start Time: "),
                new Text(_process.StartTime.ToString("O"))
            });

            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();

            try
            {
                _process.WaitForExit();
                stopwatch.Stop();
                _log.Trace(() => new[]
                {
                    new Text("Finished", Color.Header),
                    Text.NewLine,
                    new Text("Id: "),
                    new Text(_process.Id.ToString(), Color.Header),
                    Text.NewLine,
                    new Text("Process Name: "),
                    _process.ProcessName,
                    Text.NewLine,
                    new Text("Exit Code: "),
                    new Text(_process.ExitCode.ToString()),
                    Text.NewLine,
                    new Text("Exit Time: "),
                    new Text(_process.ExitTime.ToString("O")),
                    Text.NewLine,
                    new Text("Duration: "),
                    new Text(stopwatch.Elapsed.ToString("G"), Color.Header)
                });
            }
            finally
            {
                _process.OutputDataReceived -= OnOutputDataReceived;
                _process.ErrorDataReceived -= OnErrorDataReceived;
            }

            ExitCode exitCode = _process.ExitCode;
            listener.OnExitCode(exitCode);
            return exitCode;
        }

        public void Dispose() => _process.Dispose();
    }
}