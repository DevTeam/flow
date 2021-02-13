namespace Flow.Core
{
    using System.Collections.Generic;

    internal readonly struct DockerWrapperInfo
    {
        public readonly DockerImage Image;
        public readonly IEnumerable<DockerVolume> Volumes;
        public readonly IEnumerable<CommandLineArgument> Arguments;
        public readonly IEnumerable<EnvironmentVariable> EnvironmentVariables;
        public readonly OperatingSystem Platform;
        public readonly bool AutomaticallyRemove;
        public readonly DockerPullType Pull;

        public DockerWrapperInfo(
            DockerImage image,
            IEnumerable<DockerVolume> volumes,
            IEnumerable<CommandLineArgument> arguments,
            IEnumerable<EnvironmentVariable> environmentVariables,
            OperatingSystem platform,
            bool automaticallyRemove,
            DockerPullType pull)
        {
            Image = image;
            Volumes = volumes;
            Arguments = arguments;
            EnvironmentVariables = environmentVariables;
            Platform = platform;
            AutomaticallyRemove = automaticallyRemove;
            Pull = pull;
        }
    }
}
