namespace Flow.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using Moq;
    using Shouldly;
    using Xunit;

    public class DockerEnvironmentArgumentsProviderTests
    {
        private readonly Path _envFilePath = "abc";
        private readonly Mock<IFileSystem> _fileSystem;
        private readonly Dictionary<Path, string> _fileContent = new Dictionary<Path, string>();

        public DockerEnvironmentArgumentsProviderTests()
        {
            _fileSystem = new Mock<IFileSystem>();
            _fileSystem.Setup(i => i.WriteLines(It.IsAny<Path>(), It.IsAny<IEnumerable<string>>()))
                .Callback<Path, IEnumerable<string>>((path, lines) =>
                {
                    _fileContent[path] = string.Join("|", lines);
                });
        }

        [Theory]
        [InlineData("", null, "")]
        [InlineData("var1=val1", "var1=\"val1\"", "--env-file abc_env.list")]
        [InlineData("var1=val1 var2=val2", "var1=\"val1\"|var2=\"val2\"", "--env-file abc_env.list")]
        public void ShouldWrap(
            string envVars,
            string expectedEnvFile,
            string expectedArgs)
        {
            // Given
            var provider = CreateInstance();
            var wrapperInfo = new DockerWrapperInfo(
                "mcr.microsoft.com/windows/servercore",
                Enumerable.Empty<DockerVolume>(),
                Enumerable.Empty<CommandLineArgument>(),
                (Enumerable<EnvironmentVariable>)envVars,
                OperatingSystem.Windows,
                true,
                DockerPullType.Missing);

            var processInfo = ProcessInfoExtensions.Create();

            // When
            var actualArgs = string.Join(" ", 
                provider.GetArguments(wrapperInfo, processInfo).Select(i => i.Value));

            // Then
            actualArgs.ShouldBe(expectedArgs);
            _fileContent.TryGetValue("abc_env.list", out var actualEnvFile);
            actualEnvFile.ShouldBe(expectedEnvFile);
        }

        private DockerEnvironmentArgumentsProvider CreateInstance() => 
            new DockerEnvironmentArgumentsProvider(
                _fileSystem.Object,
                _envFilePath);
    }
}