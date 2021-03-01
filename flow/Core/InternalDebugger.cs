namespace Flow.Core
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using IoC;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class InternalDebugger : IDebugger
    {
        [NotNull] private readonly IStdOut _stdOut;
        private readonly bool _debug;

        public InternalDebugger(
            [NotNull] IEnvironment environment,
            [NotNull] IStdOut stdOut)
        {
            if (environment == null) throw new ArgumentNullException(nameof(environment));
            _stdOut = stdOut ?? throw new ArgumentNullException(nameof(stdOut));
            var debugStr = environment.Variables.Where(i => i.Name.ToUpperInvariant() == "FLOW_DEBUG").Select(i => i.Value.ToLowerInvariant()).SingleOrDefault() ?? string.Empty;
            if (bool.TryParse(debugStr, out var debug))
            {
                _debug = debug;
            }
        }

        public void Debug()
        {
            if (!_debug)
            {
                return;
            }

            _stdOut.WriteLine("");
            _stdOut.WriteLine($"Waiting for debugger in process: [{ Process.GetCurrentProcess().Id}] \"{Process.GetCurrentProcess().ProcessName}\"");
            _stdOut.WriteLine("");
            while (!Debugger.IsAttached)
            {
                Thread.Sleep(100);
            }

            Debugger.Break();
        }
    }
}