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

            // Environment
            yield return container
                .Bind<Path>().Tag(WorkingDirectory).To(ctx => Environment.CurrentDirectory)
                .Bind<Path>().Tag(TempDirectory).To(ctx => System.IO.Path.GetTempPath())
                .Bind<Path>().Tag(TempFile).To(ctx => new Path(System.IO.Path.Combine(
                    ctx.Container.Inject<Path>(TempDirectory).Value,
                    Guid.NewGuid().ToString().Replace("-", string.Empty))))
                .Bind<PlatformID>().To(ctx => Environment.OSVersion.Platform);

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
                .Bind<IInitializableProcessWrapper<DockerWrapperInfo>, IProcessWrapper>().Tag(Docker).To<DockerProcessWrapper>();

            // Command Line
            yield return container
                .Bind<IInitializableProcessWrapper<Path>, IProcessWrapper>().Tag(Script).To(ctx => ctx.Container.Inject<IInitializableProcessWrapper<Path>>(ctx.Container.Inject<IEnvironment>().OperatingSystem.Platform))
                .Bind<IInitializableProcessWrapper<Path>, IProcessWrapper>().Tag(PlatformID.Win32NT).Tag(PlatformID.Win32Windows).To<CmdProcessWrapper>()
                .Bind<IInitializableProcessWrapper<Path>, IProcessWrapper>().Tag(PlatformID.Unix).Tag(PlatformID.MacOSX).To<ShProcessWrapper>();
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
