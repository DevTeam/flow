namespace Flow.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using IoC;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class DockerVolumesArgumentsProvider : IDockerArgumentsProvider
    {
        [NotNull] private readonly IPathNormalizer _pathNormalizer;
        private readonly Path _workingDirectory;
        private readonly Path _tempDirectory;

        public DockerVolumesArgumentsProvider(
            [NotNull] IPathNormalizer pathNormalizer,
            [Tag(Tags.WorkingDirectory)] Path workingDirectory,
            [Tag(Tags.TempDirectory)] Path tempDirectory)
        {
            _pathNormalizer = pathNormalizer ?? throw new ArgumentNullException(nameof(pathNormalizer));
            _workingDirectory = workingDirectory;
            _tempDirectory = tempDirectory;
        }

        public IEnumerable<CommandLineArgument> GetArguments(DockerWrapperInfo wrapperInfo, ProcessInfo processInfo)
        {
            foreach (var volume in GetVolumes(wrapperInfo, processInfo))
            {
                yield return "-v";
                yield return $"{volume.HostPath.Value}:{_pathNormalizer.Normalize(volume.ContainerPath, wrapperInfo.Platform).Value}";
            }
        }

        private IEnumerable<DockerVolume> GetVolumes(DockerWrapperInfo wrapperInfo, ProcessInfo processInfo)
        {
            var values = new[]
            {
                _tempDirectory.Value,
                _workingDirectory.Value,
                processInfo.WorkingDirectory.Value
            };

            return values
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Select(value => new DockerVolume(value, value))
                .Concat(wrapperInfo.Volumes)
                .Distinct();
        }
    }
}