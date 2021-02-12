namespace Flow.Tests
{
    using Core;
    using Moq;
    using Xunit;

    public class DockerWrapperServiceTests
    {
        private readonly Mock<IProcessChain> _processChain;
        private readonly Mock<IInitializableProcessWrapper<DockerWrapperInfo>> _processWrapper;

        public DockerWrapperServiceTests()
        {
            _processChain = new Mock<IProcessChain>();
            _processWrapper = new Mock<IInitializableProcessWrapper<DockerWrapperInfo>>();
        }

        [Fact]
        public void ShouldAddDockerWrapperToChain()
        {
            // Given
            DockerImage dockerImage = "mcr.microsoft.com/windows/servercore";
            var wrapperInfo = new DockerWrapperInfo(dockerImage);
            var wrapper = CreateInstance();

            // When
            var wrapperToken = wrapper.Using(wrapperInfo);

            // Then
            _processWrapper.Verify(i => i.Initialize(wrapperInfo));
            _processChain.Verify(i => i.Append(_processWrapper.Object));
        }

        private DockerWrapperService CreateInstance() =>
            new DockerWrapperService(_processChain.Object, () => _processWrapper.Object);
    }
}
