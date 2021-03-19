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
        [NotNull] private readonly Func<IPathNormalizer> _pathNormalizer;

        public DockerArgumentsProvider(
            [NotNull, Tag(DockerEnvironment)] IDockerArgumentsProvider dockerEnvironmentArgumentsProvider,
            [NotNull, Tag(DockerVolumes)] IDockerArgumentsProvider dockerVolumesArgumentsProvider,
            [NotNull] Func<IPathNormalizer> pathNormalizer)
        {
            _dockerEnvironmentArgumentsProvider = dockerEnvironmentArgumentsProvider ?? throw new ArgumentNullException(nameof(dockerEnvironmentArgumentsProvider));
            _dockerVolumesArgumentsProvider = dockerVolumesArgumentsProvider ?? throw new ArgumentNullException(nameof(dockerVolumesArgumentsProvider));
            _pathNormalizer = pathNormalizer ?? throw new ArgumentNullException(nameof(pathNormalizer));
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

            switch (wrapperInfo.Pull)
            {
                case DockerPullType.Missing:
                    break;

                case DockerPullType.Always:
                    yield return "--pull";
                    yield return "always";
                    break;

                case DockerPullType.Never:
                    yield return "--pull";
                    yield return "never";
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!processInfo.WorkingDirectory.IsEmpty)
            {
                yield return $"--workdir={_pathNormalizer().Normalize(processInfo.WorkingDirectory)}";
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