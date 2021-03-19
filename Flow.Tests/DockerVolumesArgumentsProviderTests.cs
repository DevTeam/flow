namespace Flow.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using Moq;
    using Shouldly;
    using Xunit;
    using OperatingSystem = OperatingSystem;

    public class DockerVolumesArgumentsProviderTests
    {
        private static readonly Path DefaultWorkingDirectory = new Path("defaultWd");
        private static readonly Path DefaultTempDirectory = new Path("defaultTmp");

        private readonly Mock<IFileSystem> _fileSystem;
        private readonly Mock<Func<IPathNormalizer>> _pathNormalizer;

        public DockerVolumesArgumentsProviderTests()
        {
            _fileSystem = new Mock<IFileSystem>();
            _fileSystem.Setup(i => i.DirectoryExists(It.IsAny<Path>())).Returns(true);
            var pathNormalizer = new Mock<IPathNormalizer>();
            pathNormalizer.Setup(i => i.Normalize(It.IsAny<Path>())).Returns<Path>(i => $"n_{i.Value}");
            _pathNormalizer = new Mock<Func<IPathNormalizer>>();
            _pathNormalizer.Setup(i => i()).Returns(pathNormalizer.Object);
        }

        public static IEnumerable<object[]> TestData => new List<object[]>
        {
            new object[]
            {
                new DockerWrapperInfo(
                    new DockerImage("myImage"),
                    new []{new DockerVolume("from", "to")},
                    new CommandLineArgument[0],
                    new EnvironmentVariable[0],
                    OperatingSystem.Windows,
                    true,
                    DockerPullType.Always),
                new ProcessInfo(
                    "abc",
                    "wd",
                    new CommandLineArgument[0],
                    new EnvironmentVariable[0]),
                new[]
                {
                    "-v",
                    new CommandLineArgument($"{System.IO.Path.GetFullPath(DefaultTempDirectory.Value)}:n_{System.IO.Path.GetFullPath(DefaultTempDirectory.Value)}"),
                    "-v",
                    new CommandLineArgument($"{System.IO.Path.GetFullPath(DefaultWorkingDirectory.Value)}:n_{System.IO.Path.GetFullPath(DefaultWorkingDirectory.Value)}"),
                    "-v",
                    new CommandLineArgument($"{System.IO.Path.GetFullPath("wd")}:n_{System.IO.Path.GetFullPath("wd")}"),
                    "-v",
                    new CommandLineArgument("from:n_to")
                }
            }
        };

        [Theory]
        [MemberData(nameof(TestData))]
        internal void ShouldGetArguments(DockerWrapperInfo wrapperInfo, ProcessInfo processInfo, CommandLineArgument[] expectedArgs)
        {
            // Given
            var provider = CreateInstance();

            // When
            var actualArgs = provider.GetArguments(wrapperInfo, processInfo).ToArray();

            // Then
            actualArgs.ShouldBe(expectedArgs);
        }

        private DockerVolumesArgumentsProvider CreateInstance() => 
            new DockerVolumesArgumentsProvider(
                _pathNormalizer.Object,
                DefaultWorkingDirectory,
                DefaultTempDirectory,
                _fileSystem.Object);
    }
}
