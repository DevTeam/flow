namespace Flow.Core
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using IoC;

    // ReSharper disable once ClassNeverInstantiated.Global
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
                }
            }

            var basePath = System.IO.Path.GetFullPath(System.IO.Path.Combine(Assembly.GetAssembly(GetType()).Location, "..", ".flow"));
            MSBuildLogger = new Path(System.IO.Path.Combine(basePath, "msbuild15", "TeamCity.MSBuild.Logger.dll"));
            VSTestLogger = new Path(System.IO.Path.Combine(basePath, "vstest15", "TeamCity.VSTest.TestAdapter.dll"));
        }

        public bool IsUnderTeamCity => !string.IsNullOrWhiteSpace(Version);

        public string Version { get; }

        public Path MSBuildLogger { get; }

        public Path VSTestLogger { get; }
    }
}