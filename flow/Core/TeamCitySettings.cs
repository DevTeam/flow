namespace Flow.Core
{
    using System;
    using System.Collections.Generic;
    using IoC;

    internal class TeamCitySettings : ITeamCitySettings
    {
        public TeamCitySettings([NotNull] IEnumerable<EnvironmentVariable> variables)
        {
            if (variables == null) throw new ArgumentNullException(nameof(variables));

            foreach (var variable in variables)
            {
                switch (variable.Name.ToUpperInvariant())
                {
                    case "TEAMCITY_VERSION":
                        Version = variable.Value;
                        break;

                    case "TEAMCITY_MSBUILD_LOGGER":
                        MSBuildLogger = new Path(variable.Value);
                        break;

                    case "TEAMCITY_VSTEST_LOGGER":
                        VSTestLogger = new Path(variable.Value);
                        break;
                }
            }

            IsUnderTeamCity = !MSBuildLogger.IsEmpty && !VSTestLogger.IsEmpty;
        }

        public bool IsUnderTeamCity { get; }

        public string Version { get; set; }

        public Path MSBuildLogger { get; }

        public Path VSTestLogger { get; }
    }
}