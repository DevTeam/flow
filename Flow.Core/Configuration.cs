namespace Flow.Core
{
    using System;
    using System.Collections.Generic;
    using IoC;
    using Services;
    using static IoC.Lifetime;

    internal class Configuration: IConfiguration
    {
        public IEnumerable<IToken> Apply(IMutableContainer container)
        {
            if (IsUnderTeamCity)
            {
                yield return container
                    .Bind<IStdOut>().As(Singleton).To<TeamCityStdOut>()
                    .Bind<IStdErr>().As(Singleton).To<TeamCityStdErr>();
            }
            else
            {
                yield return container
                    .Bind<IStdOut>().As(Singleton).To<StdOut>()
                    .Bind<IStdErr>().As(Singleton).To<StdErr>();
            }
        }

        private bool IsUnderTeamCity => Environment.GetEnvironmentVariable("TEAMCITY_PROJECT_NAME") != null || Environment.GetEnvironmentVariable("TEAMCITY_VERSION") != null;
    }
}
