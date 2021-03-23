namespace Flow.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Core;
    using Moq;
    using Shouldly;
    using Xunit;
    using OperatingSystem = OperatingSystem;

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public class DockerArgumentsProviderTests
    {
        private static readonly CommandLineArgument EnvArg = new CommandLineArgument("dockerArgs");
        private static readonly CommandLineArgument VolArg = new CommandLineArgument("volArgs");

        private readonly Mock<IDockerArgumentsProvider> _dockerEnvironmentArgumentsProvider;
        private readonly Mock<IDockerArgumentsProvider> _dockerVolumesArgumentsProvider;
        private readonly Mock<Func<IPathNormalizer>> _pathNormalizer;

        public DockerArgumentsProviderTests()
        {
            _dockerEnvironmentArgumentsProvider = new Mock<IDockerArgumentsProvider>();
            _dockerVolumesArgumentsProvider = new Mock<IDockerArgumentsProvider>();
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
                    new []{new CommandLineArgument("dockerArg1")},
                    new []{new EnvironmentVariable("dockerEnv1", "dockerVal1")},
                    OperatingSystem.Windows,
                    true,
                    DockerPullType.Always),
                new ProcessInfo(
                    "abc",
                    "wd",
                    new []{new CommandLineArgument("arg1")},
                    new []{new EnvironmentVariable("env1", "val1")}),
                new[]
                {
                    "run",
                    "-it",
                    "--rm",
                    "--platform",
                    "windows",
                    "--pull",
                    "always",
                    "--workdir=n_wd",
                    EnvArg,
                    VolArg,
                    "dockerArg1",
                    "myImage",
                    "abc",
                    "arg1"
                }
            },
            new object[]
            {
                new DockerWrapperInfo(
                    new DockerImage("myImage"),
                    new []{new DockerVolume("from", "to")},
                    new []{new CommandLineArgument("dockerArg1")},
                    new []{new EnvironmentVariable("dockerEnv1", "dockerVal1")},
                    OperatingSystem.Unix,
                    false,
                    DockerPullType.Never),
                new ProcessInfo(
                    "abc",
                    new Path(),
                    new []{new CommandLineArgument("arg1")},
                    new []{new EnvironmentVariable("env1", "val1")}),
                new[]
                {
                    "run",
                    "-it",
                    "--platform",
                    "linux",
                    "--pull",
                    "never",
                    EnvArg,
                    VolArg,
                    "dockerArg1",
                    "myImage",
                    "abc",
                    "arg1"
                }
            },
            new object[]
            {
                new DockerWrapperInfo(
                    new DockerImage("myImage"),
                    new []{new DockerVolume("from", "to")},
                    new []{new CommandLineArgument("dockerArg1")},
                    new []{new EnvironmentVariable("dockerEnv1", "dockerVal1")},
                    OperatingSystem.Windows,
                    true,
                    DockerPullType.Missing),
                new ProcessInfo(
                    "abc",
                    "wd",
                    new []{new CommandLineArgument("arg1")},
                    new []{new EnvironmentVariable("env1", "val1")}),
                new[]
                {
                    "run",
                    "-it",
                    "--rm",
                    "--platform",
                    "windows",
                    "--workdir=n_wd",
                    EnvArg,
                    VolArg,
                    "dockerArg1",
                    "myImage",
                    "abc",
                    "arg1"
                }
            }
        };

        [Theory]
        [MemberData(nameof(TestData))]
        internal void ShouldGetArguments(DockerWrapperInfo wrapperInfo, ProcessInfo processInfo, CommandLineArgument[] expectedArgs)
        {
            // Given
            var provider = CreateInstance();
            _dockerEnvironmentArgumentsProvider.Setup(i => i.GetArguments(wrapperInfo, processInfo)).Returns(new[] { EnvArg });
            _dockerVolumesArgumentsProvider.Setup(i => i.GetArguments(wrapperInfo, processInfo)).Returns(new[] { VolArg });

            // When
            var actualArgs = provider.GetArguments(wrapperInfo, processInfo).ToArray();

            // Then
            actualArgs.ShouldBe(expectedArgs);
        }

        private DockerArgumentsProvider CreateInstance() => 
            new DockerArgumentsProvider(
                _dockerEnvironmentArgumentsProvider.Object,
                _dockerVolumesArgumentsProvider.Object,
                _pathNormalizer.Object);
    }
}
