namespace Flow.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using IoC;
    using OperatingSystem = OperatingSystem;
    using static Tags;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class DockerArgumentsProvider : IDockerArgumentsProvider
    {
        private readonly IDockerArgumentsProvider _dockerEnvironmentArgumentsProvider;
        [NotNull] private readonly IDockerArgumentsProvider _dockerVolumesArgumentsProvider;

        public DockerArgumentsProvider(
            [NotNull, Tag(DockerEnvironment)] IDockerArgumentsProvider dockerEnvironmentArgumentsProvider,
            [NotNull, Tag(DockerVolumes)] IDockerArgumentsProvider dockerVolumesArgumentsProvider)
        {
            _dockerEnvironmentArgumentsProvider = dockerEnvironmentArgumentsProvider ?? throw new ArgumentNullException(nameof(dockerEnvironmentArgumentsProvider));
            _dockerVolumesArgumentsProvider = dockerVolumesArgumentsProvider ?? throw new ArgumentNullException(nameof(dockerVolumesArgumentsProvider));
        }

        public IEnumerable<CommandLineArgument> GetArguments(DockerWrapperInfo wrapperInfo, ProcessInfo processInfo)
        {
            yield return "run";
            yield return "-it";
            if (wrapperInfo.AutomaticallyRemove)
            {
                yield return "--rm";
            }

            yield return "--platform";
            switch (wrapperInfo.Platform)
            {
                case OperatingSystem.Windows:
                    yield return "windows";
                    break;

                case OperatingSystem.Unix:
                    yield return "linux";
                    break;

                case OperatingSystem.Mac:
                    throw new NotSupportedException();

                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (wrapperInfo.Pull != DockerPullType.Missing)
            {
                yield return "--pull";
                switch (wrapperInfo.Pull)
                {
                    case DockerPullType.Missing:
                        break;

                    case DockerPullType.Always:
                        yield return "always";
                        break;

                    case DockerPullType.Never:
                        yield return "never";
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            var args =
                new[] {_dockerEnvironmentArgumentsProvider, _dockerVolumesArgumentsProvider}
                    .SelectMany(i => i.GetArguments(wrapperInfo, processInfo));

            foreach (var arg in args)
            {
                yield return arg;
            }

            foreach (var argument in wrapperInfo.Arguments)
            {
                yield return argument;
            }

            yield return wrapperInfo.Image.Name;
            yield return processInfo.Executable.Value;
            foreach (var arg in processInfo.Arguments)
            {
                yield return arg;
            }
        }
    }
}