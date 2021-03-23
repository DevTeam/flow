namespace Flow.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using Moq;
    using Shouldly;
    using Xunit;

    public class DockerProcessWrapperTests
    {
        private readonly Mock<IDockerArgumentsProvider> _dockerArgumentsProvider;

        public DockerProcessWrapperTests()
        {
            _dockerArgumentsProvider = new Mock<IDockerArgumentsProvider>();
        }

        [Fact]
        public void ShouldWrap()
        {
            // Given
            var wrapperInfo = DockerWrapperInfoExtensions.Create();
            var wrapper = CreateInstance(wrapperInfo);
            var processInfo = ProcessInfoExtensions.Create();

            // When
            var actualProcessInfo = wrapper.Wrap(processInfo);

            // Then
            actualProcessInfo.Executable.ShouldBe("docker");
            actualProcessInfo.WorkingDirectory.ShouldBe("wd");
            actualProcessInfo.Variables.ToList().ShouldBe(new List<EnvironmentVariable>() { "var1=val1", "var2=val2" });
        }

        private DockerProcessWrapper CreateInstance(DockerWrapperInfo wrapperInfo) => 
            new DockerProcessWrapper(_dockerArgumentsProvider.Object, wrapperInfo);
    }
}