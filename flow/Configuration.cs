namespace Flow
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Core;
    using IoC;
    using static IoC.Lifetime;

    internal class Configuration: IConfiguration
    {
        public IEnumerable<IToken> Apply(IMutableContainer container)
        {
            var autowiringStrategy = AutowiringStrategies
                .AspectOriented()
                .Tag<TagAttribute>(i => i.Tag);

            container.Bind<IAutowiringStrategy>().To(ctx => autowiringStrategy);

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

            yield return container
                .Bind<IProcessFactory>().As(Singleton).To<ProcessFactory>()
                .Bind<IConverter<CommandLineArgument, string>>().As(Singleton).To<ArgumentToStringConverter>()
                .Bind<IEnvironment>().As(Singleton).To<InternalEnvironment>()
                .Bind<IProcess>().To<InternalProcess>(ctx => new InternalProcess(Arg<Process, IProcess>(ctx.Args, "process")))
                .Bind<IProcessListener>().As(Singleton).Tag("stdOutErr").To<ProcessStdOutErrListener>();

            yield return container
                .Bind<ICommandLineService>().As(Singleton).To<CommandLineService>();
        }

        private static bool IsUnderTeamCity =>
            Environment.GetEnvironmentVariable("TEAMCITY_PROJECT_NAME") != null
            || Environment.GetEnvironmentVariable("TEAMCITY_VERSION") != null;

        private static TArg Arg<TArg, TTarget>(object[] args, string name) => 
            args.Length == 1 && args[0] is TArg arg
                ? arg
                : throw new ArgumentException($"Please resolve using Func<{nameof(TArg)}, {nameof(TTarget)}>.", name);
    }
}
