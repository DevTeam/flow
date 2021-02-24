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
                .Bind<OperatingSystem>().Tag(Base).To(ctx => OperatingSystem)
                .Bind<OperatingSystem>().To(ctx => ctx.Container.Inject<IChain<OperatingSystem>>().Current)
                .Bind<IPathNormalizer>().Tag(Base).To<BasePathNormalizer>()
                .Bind<IPathNormalizer>().To(ctx => ctx.Container.Inject<IChain<IPathNormalizer>>().Current)
                .Bind<ILocator>().As(Singleton).To<Locator>(ctx => ctx.Container.Assign(ctx.It.SearchTool, ctx.Container.Inject<OperatingSystem>() == OperatingSystem.Windows ? "where.exe" : "which"))
                .Bind<IToolResolver>().As(Singleton).To<ToolResolver>()
                .Bind<bool>().Tag(TeamCity).To(ctx =>
                    Environment.GetEnvironmentVariable("TEAMCITY_PROJECT_NAME") != null
                    || Environment.GetEnvironmentVariable("TEAMCITY_VERSION") != null)
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
                .Bind<IStdOut>().To(ctx => ctx.Container.Inject<IStdOut>(ctx.Container.Inject<bool>(TeamCity) ? TeamCity : Base))
                .Bind<IStdOut>().As(Singleton).Tag(Base).To<TeamCityStdOut>()
                .Bind<IStdOut>().As(Singleton).Tag(TeamCity).To<TeamCityStdOut>()
                .Bind<IStdErr>().To(ctx => ctx.Container.Inject<IStdErr>(ctx.Container.Inject<bool>(TeamCity) ? TeamCity : Base))
                .Bind<IStdErr>().As(Singleton).Tag(Base).To<TeamCityStdErr>()
                .Bind<IStdErr>().As(Singleton).Tag(TeamCity).To<TeamCityStdErr>()
                .Bind<IFileSystem>().As(Singleton).To<FileSystem>()

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
                .Bind<IDotnetWrapperService>().To<DotnetWrapperService>()
                .Bind<IDotnetBuildService>().To<DotnetBuildService>();
        }

        private static TArg Arg<TArg, TTarget>(object[] args, string name) => 
            args.Length == 1 && args[0] is TArg arg
                ? arg
                : throw new ArgumentException($"Please resolve using Func<{nameof(TArg)}, {nameof(TTarget)}>.", name);

        private OperatingSystem OperatingSystem
        {
            get
            {
                switch (Environment.OSVersion.Platform)
                {
                    case PlatformID.Win32S:
                    case PlatformID.Win32Windows:
                    case PlatformID.Win32NT:
                        return OperatingSystem.Windows;

                    case PlatformID.Unix:
                        return OperatingSystem.Unix;

                    case PlatformID.MacOSX:
                        return OperatingSystem.Mac;

                    case PlatformID.WinCE:
                    case PlatformID.Xbox:
                        throw new NotSupportedException();

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
