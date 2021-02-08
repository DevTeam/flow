namespace Flow
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Core;
    using IoC;
    using static Core.Tags;
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

            // Directories
            yield return container
                .Bind<Path>().Tag(WorkingDirectory).To(ctx => Environment.CurrentDirectory)
                .Bind<Path>().Tag(TempDirectory).To(ctx => System.IO.Path.GetTempPath());

            // Processes
            yield return container
                .Bind<ICommandLineService>().As(Singleton).To<CommandLineService>()
                .Bind<IProcessFactory>().Bind<IProcessChain>().As(Singleton).Tag(Composite).Tag().To<CompositeProcessFactory>()
                .Bind<IProcessFactory>().As(Singleton).Tag(Base).To<ProcessFactory>()
                .Bind<IConverter<CommandLineArgument, string>>().As(Singleton).To<ArgumentToStringConverter>()
                .Bind<IEnvironment>().As(Singleton).To<InternalEnvironment>()
                .Bind<IProcess>().To<InternalProcess>(ctx => new InternalProcess(Arg<Process, IProcess>(ctx.Args, "process")))
                .Bind<IProcessListener>().As(Singleton).Tag(StdOutErr).To<ProcessStdOutErrListener>();

            // Docker
            yield return container
                .Bind<IDockerWrapperService>().As(Singleton).Tag().To<DockerWrapperService>()
                .Bind<IProcessWrapper>().As(Singleton).Tag(Docker).To<DockerProcessWrapper>();
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
