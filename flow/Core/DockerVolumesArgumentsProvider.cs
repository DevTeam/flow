namespace Flow.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using IoC;
    
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class DockerVolumesArgumentsProvider : IDockerArgumentsProvider
    {
        [NotNull] private readonly Func<IPathNormalizer> _pathNormalizer;
        private readonly Path _workingDirectory;
        private readonly Path _tempDirectory;
        [NotNull] private readonly IFileSystem _fileSystem;

        public DockerVolumesArgumentsProvider(
            [NotNull] Func<IPathNormalizer> pathNormalizer,
            [Tag(Tags.WorkingDirectory)] Path workingDirectory,
            [Tag(Tags.TempDirectory)] Path tempDirectory,
            [NotNull] IFileSystem fileSystem)
        {
            _pathNormalizer = pathNormalizer ?? throw new ArgumentNullException(nameof(pathNormalizer));
            _workingDirectory = workingDirectory;
            _tempDirectory = tempDirectory;
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        public IEnumerable<CommandLineArgument> GetArguments(DockerWrapperInfo wrapperInfo, ProcessInfo processInfo)
        {
            foreach (var volume in GetVolumes(wrapperInfo, processInfo))
            {
                yield return "-v";
                yield return $"{volume.HostPath.Value}:{_pathNormalizer().Normalize(volume.ContainerPath).Value}";
            }
        }

        private IEnumerable<DockerVolume> GetVolumes(DockerWrapperInfo wrapperInfo, ProcessInfo processInfo)
        {
            var baseValues = new []
            {
                _tempDirectory,
                _workingDirectory,
                processInfo.WorkingDirectory
            };

            return baseValues
                .Where(i => !i.IsEmpty)
                .Select(i => i.Value)
                .Distinct()
                .Select(System.IO.Path.GetFullPath)
                .Where(i => !string.IsNullOrWhiteSpace(i))
                .Select(i => new Path(i))
                .Where(_fileSystem.DirectoryExists)
                .Select(i => i.Value)
                .Select(value => new DockerVolume(value, value))
                .Concat(wrapperInfo.Volumes)
                .Distinct();
        }
    }
}