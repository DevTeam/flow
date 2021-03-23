namespace Flow.Tests
{
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Core;

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    internal class DockerWrapperInfoExtensions
    {
        public static DockerWrapperInfo Create()
        {
            return new DockerWrapperInfo(
                "mcr.microsoft.com/windows/servercore",
                Enumerable.Empty<DockerVolume>(),
                Enumerable.Empty<CommandLineArgument>(),
                Enumerable.Empty<EnvironmentVariable>(),
                OperatingSystem.Unix,
                true,
                DockerPullType.Missing);
        }

    }
}
