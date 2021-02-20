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

            yield return container
                .Bind<IAutowiringStrategy>().To(ctx => autowiringStrategy)

                // Settings
                .Bind<IEnvironment>().As(Singleton).To<InternalEnvironment>()
                .Bind<OperatingSystem>().To(ctx => ctx.Container.Inject<IEnvironment>().OperatingSystem)
                .Bind<bool>().Tag(TeamCity).To(ctx => ctx.Container.Inject<IEnvironment>().IsUnderTeamCity)
                .Bind<char>().Tag(SeparatorChar).To(ctx => ' ')
                .Bind<char>().Tag(QuoteChar).To(ctx => '\"')
                .Bind<Path>().As(Singleton).Tag(DotnetExecutable).To(ctx => ctx.Container.Inject<IEnvironment>().DotnetExecutable)
                .Bind<string>().Tag(WindowsNewLineString).To(ctx => "\r\n")
                .Bind<string>().Tag(LinuxNewLineString).To(ctx => "\n")
                .Bind<string>().Tag(FlowVersionString).To(ctx => GetType().Assembly.GetName().Version.ToString())
                .Bind<string>().Tag(WindowsDirectorySeparatorString).To(ctx => "\\")
                .Bind<string>().Tag(LinuxDirectorySeparatorString).To(ctx => "/")
                .Bind<string>().Tag(WslRootString).To(ctx => "/mnt/c/")
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
                .Bind<IPathNormalizer>().As(Singleton).To<PathNormalizer>()

                // Command Line
                .Bind<ICommandLineService>().As(Singleton).To<CommandLineService>()
                .Bind<IProcessFactory>().Bind<IProcessChain>().As(Singleton).Tag().To<CompositeProcessFactory>()
                .Bind<IProcessFactory>().As(Singleton).Tag(Base).To<ProcessFactory>()
                .Bind<IConverter<CommandLineArgument, string>>().As(Singleton).To<ArgumentToStringConverter>()
                .Bind<IConverter<IEnumerable<CommandLineArgument>, string>>().As(Singleton).To<ArgumentsToStringConverter>()
                .Bind<IProcess>().To<InternalProcess>(ctx => new InternalProcess(Arg<Process, IProcess>(ctx.Args, "process")))
                .Bind<IProcessListener>().As(Singleton).Tag(StdOutErr).To<ProcessStdOutErrListener>()

                // Process Wrappers
                .Bind<IProcessWrapper>().As(Singleton).Tag(CmdScriptWrapper).To<CmdProcessWrapper>()
                .Bind<IProcessWrapper>().As(Singleton).Tag(ShScriptWrapper).To<ShProcessWrapper>()
                .Bind<IProcessWrapper>().As(Singleton).Tag(WslScriptWrapper).To<ShProcessWrapper>(ctx => ctx.Container.Assign(ctx.It.OperatingSystem, OperatingSystem.Wsl))
                .Bind<IProcessWrapper>().Tag(DockerWrapper).To<DockerProcessWrapper>(ctx => ctx.It.Initialize(Arg<DockerWrapperInfo, DockerProcessWrapper>(ctx.Args, "info")))

                // Docker Wrapper
                .Bind<IDockerWrapperService>().As(Singleton).To<DockerWrapperService>()
                .Bind<IDockerArgumentsProvider>().As(Singleton).To<DockerArgumentsProvider>()
                .Bind<IDockerArgumentsProvider>().As(Singleton).Tag(DockerEnvironment).To<DockerEnvironmentArgumentsProvider>()
                .Bind<IDockerArgumentsProvider>().As(Singleton).Tag(DockerVolumes).To<DockerVolumesArgumentsProvider>()

                // Wsl
                .Bind<IWslWrapperService>().As(Singleton).To<WslWrapperService>()

                // Dotnet
                .Bind<IDotnetWrapperService>().To<DotnetWrapperService>()
                .Bind<IDotnetBuildService>().As(Singleton).To<DotnetBuildService>();
        }

        private static TArg Arg<TArg, TTarget>(object[] args, string name) => 
            args.Length == 1 && args[0] is TArg arg
                ? arg
                : throw new ArgumentException($"Please resolve using Func<{nameof(TArg)}, {nameof(TTarget)}>.", name);
    }
}
