namespace Flow.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using IoC;
    using static Tags;

    // ReSharper disable once ClassNeverInstantiated.Global
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal class Flow : IStages
    {
        [NotNull] private readonly ILog<Flow> _log;
        [NotNull] private readonly string _version;
        [NotNull] private readonly ITeamCitySettings _teamCitySettings;
        [NotNull] private readonly IColorfulStdOut _stdOut;
        private readonly bool _debug;

        public Flow(
            [NotNull] ILog<Flow> log,
            [NotNull, Tag(FlowVersionString)] string version,
            [NotNull] ITeamCitySettings teamCitySettings,
            [NotNull] IEnvironment environment,
            [NotNull] IColorfulStdOut stdOut)
        {
            if (environment == null) throw new ArgumentNullException(nameof(environment));
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _version = version ?? throw new ArgumentNullException(nameof(version));
            _teamCitySettings = teamCitySettings ?? throw new ArgumentNullException(nameof(teamCitySettings));
            _stdOut = stdOut ?? throw new ArgumentNullException(nameof(stdOut));
            var debugStr = environment.Variables.Where(i => i.Name.ToUpperInvariant() == "FLOW_DEBUG").Select(i => i.Value.ToLowerInvariant()).SingleOrDefault() ?? string.Empty;
            if (bool.TryParse(debugStr, out var debug))
            {
                _debug = debug;
            }
        }

        public void Before()
        {
            _log.Info(() => new Text($"Flow {_version}", Color.Header) + (_teamCitySettings.IsUnderTeamCity ? $" under TeamCity {_teamCitySettings.Version}" : string.Empty));
            if (_debug)
            {
                _stdOut.WriteLine(string.Empty);
                _stdOut.WriteLine(new Text("Waiting for debugger in process: ", Color.Header), $"[{Process.GetCurrentProcess().Id}] \"{Process.GetCurrentProcess().ProcessName}\"");
                _stdOut.WriteLine(string.Empty);
                while (!Debugger.IsAttached)
                {
                    Thread.Sleep(100);
                }

                Debugger.Break();
            }
        }

        public void After(IDictionary<string, object> results)
        {
            _log.Trace(() => new Text("Completed"));
        }
    }
}