namespace Flow.Tests
{
    using Core;
    using Moq;
    using Xunit;

    public class DockerWrapperServiceTests
    {
        private readonly Mock<IProcessChain> _processChain;
        private readonly Mock<IProcessWrapper> _processWrapper;

        public DockerWrapperServiceTests()
        {
            _processChain = new Mock<IProcessChain>();
            _processWrapper = new Mock<IProcessWrapper>();
        }

        [Fact]
        public void ShouldAddDockerWrapperToChain()
        {
            // Given
            DockerImage dockerImage = "mcr.microsoft.com/windows/servercore";
            var dockerWrapperInfo = new DockerWrapperInfo(dockerImage);
            var wrapper = CreateInstance();

            // When
            var wrapperToken = wrapper.Using(dockerWrapperInfo);

            // Then
            _processWrapper.Verify(i => i.Initialize(dockerWrapperInfo));
            _processChain.Verify(i => i.Append(_processWrapper.Object));
        }

        private DockerWrapperService CreateInstance() =>
            new DockerWrapperService(_processChain.Object, () => _processWrapper.Object);
    }
}
