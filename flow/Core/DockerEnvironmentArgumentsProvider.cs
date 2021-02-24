namespace Flow.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using IoC;
    using OperatingSystem = OperatingSystem;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class DockerEnvironmentArgumentsProvider : IDockerArgumentsProvider
    {
        [NotNull] private readonly IFileSystem _fileSystem;
        private readonly Path _envFilePath;

        public DockerEnvironmentArgumentsProvider(
            [NotNull] IFileSystem fileSystem,
            [Tag(Tags.TempFile)] Path envFilePath)
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            _envFilePath = new Path(envFilePath.Value + "_env.list");
        }

        public IEnumerable<CommandLineArgument> GetArguments(DockerWrapperInfo wrapperInfo, ProcessInfo processInfo)
        {
            if (wrapperInfo.EnvironmentVariables.Any())
            {
                _fileSystem.WriteLines(_envFilePath, wrapperInfo.EnvironmentVariables.Select(i => $"{i.Name}=\"{i.Value}\""));
                yield return "--env-file";
                yield return _envFilePath.Value;
            }
        }
    }
}