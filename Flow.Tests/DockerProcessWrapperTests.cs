namespace Flow.Tests
{
    using Core;
    using Moq;
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
            var wrapper = CreateInstance();
            var wrapperInfo = DockerWrapperInfoExtensions.Create();
            wrapper.Initialize(wrapperInfo);
            var processInfo = ProcessInfoExtensions.Create();

            // When
            var actualProcessInfo = wrapper.Wrap(processInfo);

            // Then
        }

        private DockerProcessWrapper CreateInstance() => 
            new DockerProcessWrapper(_dockerArgumentsProvider.Object);
    }
}