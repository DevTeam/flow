﻿namespace Flow
{
    using System;
    using System.Collections.Generic;
    using Core;
    using IoC;
    using JetBrains.TeamCity.ServiceMessages.Read;
    using JetBrains.TeamCity.ServiceMessages.Write;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;
    using JetBrains.TeamCity.ServiceMessages.Write.Special.Impl.Updater;
    using static Core.Tags;
    using static IoC.Lifetime;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
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
                .Bind<DateTime>().To(ctx => DateTime.Now)
                .Bind<ISettings>().Bind<ISettingsSetup>().As(Singleton).To<Settings>()
                .Bind<Verbosity>().To(ctx => ctx.Container.Inject<ISettings>().Verbosity)
                .Bind<ILog<TT>>().As(Singleton).To<Log<TT>>()
                .Bind<IChain<TT>>().As(Singleton).To<Chain<TT>>()
                .Bind<IEnvironment>().Tag(Base).To<DefaultEnvironment>()
                .Bind<IVirtualEnvironment>().To<VirtualEnvironment>()
                .Bind<IEnvironment>().To(ctx => ctx.Container.Inject<IChain<IEnvironment>>().Current)
                .Bind<OperatingSystem>().To(ctx => ctx.Container.Inject<IEnvironment>().OperatingSystem)
                .Bind<IPathNormalizer>().To(ctx => ctx.Container.Inject<IEnvironment>().PathNormalizer)
                .Bind<IEnumerable<EnvironmentVariable>>().To(ctx => ctx.Container.Inject<IEnvironment>().Variables)
                .Bind<ITeamCitySettings>().As(Singleton).To<TeamCitySettings>()
                .Bind<IStages>().To<Flow>()
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
                .Bind<ICommandLineParser>().As(Singleton).To<CommandLineParser>()
                .Bind<IColorTheme>().As(Singleton).To<ColorTheme>()
                .Bind<IColorfulStdOut>().As(Singleton).To<ColorfulStdOut>()
                .Bind<IStdOut>().To(ctx => ctx.Container.Inject<IStdOut>(ctx.Container.Inject<ITeamCitySettings>().IsUnderTeamCity ? TeamCity : Base))
                .Bind<IStdOut>().As(Singleton).Tag(Base).To<TeamCityStdOut>()
                .Bind<IStdOut>().As(Singleton).Tag(TeamCity).To<TeamCityStdOut>()
                .Bind<IStdErr>().To(ctx => ctx.Container.Inject<IStdErr>(ctx.Container.Inject<ITeamCitySettings>().IsUnderTeamCity ? TeamCity : Base))
                .Bind<IStdErr>().As(Singleton).Tag(Base).To<TeamCityStdErr>()
                .Bind<IStdErr>().As(Singleton).Tag(TeamCity).To<TeamCityStdErr>()
                .Bind<IFileSystem>().As(Singleton).To<FileSystem>()
                .Bind<IConverter<MSBuildParameter, string>>().As(Singleton).To<MSBuildParameterToStringConverter>()

                // Command Line
                .Bind<ICommandLineService>().To<CommandLineService>()
                .Bind<IProcessFactory>().Bind<IChain<IProcessWrapper>>().As(Singleton).Tag().To<CompositeProcessFactory>()
                .Bind<IProcessFactory>().Tag(Base).To<ProcessFactory>()
                .Bind<IConverter<CommandLineArgument, string>>().To<ArgumentToStringConverter>()
                .Bind<IConverter<IEnumerable<CommandLineArgument>, string>>().To<ArgumentsToStringConverter>()
                .Bind<IProcess>().To<FlowProcess>()
                .Bind<IProcessListener>().Tag(StdOutErr).To<ProcessListener>()
                
                // Process Wrappers
                .Bind<IProcessWrapper>().Tag(CmdScriptWrapper).To<CmdProcessWrapper>()
                .Bind<IProcessWrapper>().Tag(ShScriptWrapper).To<ShProcessWrapper>()
                .Bind<IProcessWrapper>().Tag(WslScriptWrapper).To<ShProcessWrapper>()
                .Bind<IProcessWrapper>().Tag(DockerWrapper).To<DockerProcessWrapper>()

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
                .Bind<IProcessListener<BuildResult>>().To<BuildListener>()
                .Bind<IMessageProcessor>().To<MessageProcessor>()
                .Bind<IBuildLogFlow>().To<BuildLogFlow>()

                // TeamCity messages
                .Bind<IServiceMessageFormatter>().As(Singleton).To<ServiceMessageFormatter>()
                .Bind<IFlowIdGenerator>().As(Singleton).To<FlowIdGenerator>()
                .Bind<IServiceMessageUpdater>().As(Singleton).To<TimestampUpdater>()
                .Bind<TeamCityServiceMessages>().As(Singleton).To()
                .Bind<ITeamCityWriter>().To(ctx => ctx.Container.Inject<TeamCityServiceMessages>().CreateWriter(line => ctx.Container.Inject<IStdOut>(Base).WriteLine(line)))
                .Bind<IServiceMessageParser>().As(Singleton).To<ServiceMessageParser>();
        }
    }
}
