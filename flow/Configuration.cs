namespace Flow
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Core;
    using IoC;
    using JetBrains.TeamCity.ServiceMessages.Read;
    using JetBrains.TeamCity.ServiceMessages.Write;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;
    using JetBrains.TeamCity.ServiceMessages.Write.Special.Impl.Updater;
    using static Core.Tags;
    using static IoC.Lifetime;

    internal class Configuration: IConfiguration
    {
        internal const char CommandLineArgumentsSeparatorChar = ' ';
        internal const char CommandLineArgumentsQuoteChar = '"';

        public IEnumerable<IToken> Apply(IMutableContainer container)
        {
            var autowiringStrategy = AutowiringStrategies
                .AspectOriented()
                .Tag<TagAttribute>(i => i.Tag);

            yield return container
                .Bind<IAutowiringStrategy>().To(ctx => autowiringStrategy)
                // Settings
                .Bind<IChain<TT>>().As(Singleton).To<Chain<TT>>()
                .Bind<IEnvironment>().Tag(Base).To<DefaultEnvironment>()
                .Bind<IVirtualEnvironment>().To<VirtualEnvironment>()
                .Bind<IEnvironment>().To(ctx => ctx.Container.Inject<IChain<IEnvironment>>().Current)
                .Bind<OperatingSystem>().To(ctx => ctx.Container.Inject<IEnvironment>().OperatingSystem)
                .Bind<IPathNormalizer>().To(ctx => ctx.Container.Inject<IEnvironment>().PathNormalizer)
                .Bind<IEnumerable<EnvironmentVariable>>().To(ctx => ctx.Container.Inject<IEnvironment>().Variables)
                .Bind<ITeamCitySettings>().To<TeamCitySettings>()
                .Bind<IStages>().To<Stages>()
                .Bind<ILocator>().As(Singleton).To<Locator>(ctx => ctx.Container.Assign(ctx.It.SearchTool, ctx.Container.Inject<OperatingSystem>() == OperatingSystem.Windows ? "where.exe" : "which"))
                .Bind<IToolResolver>().As(Singleton).To<ToolResolver>()
                .Bind<char>().Tag(ArgumentsSeparatorChar).To(ctx => CommandLineArgumentsSeparatorChar)
                .Bind<char>().Tag(ArgumentsQuoteChar).To(ctx => CommandLineArgumentsQuoteChar)
                .Bind<string>().Tag(NewLineString).To(ctx => ctx.Container.Inject<OperatingSystem>() == OperatingSystem.Windows ? "\r\n" : "\n")
                .Bind<string>().Tag(FlowVersionString).To(ctx => GetType().Assembly.GetName().Version.ToString())
                .Bind<string>().Tag(DirectorySeparatorString).To(ctx => ctx.Container.Inject<OperatingSystem>() == OperatingSystem.Windows ? "\\" : "/")
                .Bind<string>().To(ctx => ctx.Container.Inject<IChain<string>>().Current)
                .Bind<Path>().Tag(WorkingDirectory).To(ctx => Environment.CurrentDirectory)
                .Bind<Path>().Tag(TempDirectory).To(ctx => System.IO.Path.GetTempPath())
                .Bind<Path>().Tag(TempFile).To(ctx => new Path(System.IO.Path.Combine(
                    ctx.Container.Inject<Path>(TempDirectory).Value,
                    Guid.NewGuid().ToString().Replace("-", string.Empty))))

                // Common
                .Bind<IStdOut>().To(ctx => ctx.Container.Inject<IStdOut>(ctx.Container.Inject<ITeamCitySettings>().IsUnderTeamCity ? TeamCity : Base))
                .Bind<IStdOut>().As(Singleton).Tag(Base).To<TeamCityStdOut>()
                .Bind<IStdOut>().As(Singleton).Tag(Tags.TeamCity).To<TeamCityStdOut>()
                .Bind<IStdErr>().To(ctx => ctx.Container.Inject<IStdErr>(ctx.Container.Inject<ITeamCitySettings>().IsUnderTeamCity ? TeamCity : Base))
                .Bind<IStdErr>().As(Singleton).Tag(Base).To<TeamCityStdErr>()
                .Bind<IStdErr>().As(Singleton).Tag(Tags.TeamCity).To<TeamCityStdErr>()
                .Bind<IFileSystem>().As(Singleton).To<FileSystem>()
                .Bind<IConverter<MSBuildParameter, string>>().As(Singleton).To<MSBuildParameterToStringConverter>()

                // Command Line
                .Bind<ICommandLineService>().To<CommandLineService>()
                .Bind<IProcessFactory>().Bind<IChain<IProcessWrapper>>().As(Singleton).Tag().To<CurrentProcessFactory>()
                .Bind<IProcessFactory>().Tag(Base).To<ProcessFactory>()
                .Bind<IConverter<CommandLineArgument, string>>().To<ArgumentToStringConverter>()
                .Bind<IConverter<IEnumerable<CommandLineArgument>, string>>().To<ArgumentsToStringConverter>()
                .Bind<IProcess>().To<InternalProcess>(ctx => new InternalProcess(Arg<Process, IProcess>(ctx.Args, "process")))
                .Bind<IProcessListener>().Tag(StdOutErr).To<ProcessStdOutErrListener>()
                
                // Process Wrappers
                .Bind<IProcessWrapper>().Tag(CmdScriptWrapper).To<CmdProcessWrapper>()
                .Bind<IProcessWrapper>().Tag(ShScriptWrapper).To<ShProcessWrapper>()
                .Bind<IProcessWrapper>().Tag(WslScriptWrapper).To<ShProcessWrapper>()
                .Bind<IProcessWrapper>().Tag(DockerWrapper).To<DockerProcessWrapper>(ctx => ctx.It.Initialize(Arg<DockerWrapperInfo, DockerProcessWrapper>(ctx.Args, "info")))

                // Docker Wrapper
                .Bind<IDockerWrapperService>().To<DockerWrapperService>()
                .Bind<IDockerArgumentsProvider>().To<DockerArgumentsProvider>()
                .Bind<IDockerArgumentsProvider>().Tag(DockerEnvironment).To<DockerEnvironmentArgumentsProvider>()
                .Bind<IDockerArgumentsProvider>().Tag(DockerVolumes).To<DockerVolumesArgumentsProvider>()

                // Wsl
                .Bind<IWslWrapperService>().To<WslWrapperService>()

                // Dotnet
                .Bind<IResponseFile>().To<MSBuildResponseFile>()
                .Bind<IDotnetWrapperService>().To<DotnetWrapperService>()
                .Bind<IDotnetBuildService>().To<DotnetBuildService>()
                .Bind<IProcessListener<BuildResult>>().To<TeamCityBuildListener>()
                .Bind<IBuildLogFlow>().To<BuildLogFlow>()

                // TeamCity messages
                .Bind<ITeamCityWriter>().To(ctx => CreateWriter(ctx.Container.Inject<IStdOut>(Base)))
                .Bind<IServiceMessageParser>().As(Singleton).To<ServiceMessageParser>();
        }

        private static TArg Arg<TArg, TTarget>(object[] args, string name) => 
            args.Length == 1 && args[0] is TArg arg
                ? arg
                : throw new ArgumentException($"Please resolve using Func<{nameof(TArg)}, {nameof(TTarget)}>.", name);

        private static ITeamCityWriter CreateWriter(IStdOut stdOut)
        {
            return new TeamCityServiceMessages(
                new ServiceMessageFormatter(),
                new FlowIdGenerator(),
                new IServiceMessageUpdater[] { new TimestampUpdater(() => DateTime.Now) }).CreateWriter(stdOut.WriteLine);
        }
    }
}
