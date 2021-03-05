﻿namespace Flow.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using IoC;
    using static Tags;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class Stages : IStages
    {
        [NotNull] private readonly string _version;
        [NotNull] private readonly ITeamCitySettings _teamCitySettings;
        [NotNull] private readonly IColorfulStdOut _stdOut;
        private readonly bool _debug;

        public Stages(
            [NotNull, Tag(FlowVersionString)] string version,
            [NotNull] ITeamCitySettings teamCitySettings,
            [NotNull] IEnvironment environment,
            [NotNull] IColorfulStdOut stdOut)
        {
            if (environment == null) throw new ArgumentNullException(nameof(environment));
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
            var header = new StringBuilder($"Flow {_version}");
            if (_teamCitySettings.IsUnderTeamCity)
            {
                header.Append($" under TeamCity {_teamCitySettings.Version}");
            }

            _stdOut.WriteLine(new Text(header.ToString(), Color.Header));
            if (_debug)
            {
                _stdOut.WriteLine(new Text(""));
                _stdOut.WriteLine(new Text("Waiting for debugger in process: ", Color.Header), new Text($"[{Process.GetCurrentProcess().Id}] \"{Process.GetCurrentProcess().ProcessName}\""));
                _stdOut.WriteLine(new Text(""));
                while (!Debugger.IsAttached)
                {
                    Thread.Sleep(100);
                }

                Debugger.Break();
            }
        }

        public void After(IDictionary<string, object> results)
        {
        }
    }
}