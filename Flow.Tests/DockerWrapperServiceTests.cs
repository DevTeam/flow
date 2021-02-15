namespace Flow.Tests
{
    using System.Linq;
    using Core;
    using Moq;
    using Xunit;

    public class DockerWrapperServiceTests
    {
        private readonly Mock<IProcessChain> _processChain;
        private readonly Mock<IInitializableProcessWrapper<DockerWrapperInfo>> _dockerProcessWrapper;
        private readonly Mock<IProcessWrapper> _cmdProcessWrapper;
        private readonly Mock<IProcessWrapper> _shProcessWrapper;

        public DockerWrapperServiceTests()
        {
            _processChain = new Mock<IProcessChain>();
            _dockerProcessWrapper = new Mock<IInitializableProcessWrapper<DockerWrapperInfo>>();
            _cmdProcessWrapper = new Mock<IProcessWrapper>();
            _shProcessWrapper = new Mock<IProcessWrapper>();
        }

        [Fact]
        public void ShouldAddDockerWrapperToChain()
        {
            // Given
            DockerImage dockerImage = "mcr.microsoft.com/windows/servercore";
            var wrapperInfo = new DockerWrapperInfo(
                dockerImage,
                Enumerable.Empty<DockerVolume>(),
                Enumerable.Empty<CommandLineArgument>(),
                Enumerable.Empty<EnvironmentVariable>(),
                OperatingSystem.Unix,
                true,
                DockerPullType.Missing);

            var wrapper = CreateInstance();

            // When
            var wrapperToken = wrapper.Using(wrapperInfo);

            // Then
            _dockerProcessWrapper.Verify(i => i.Initialize(wrapperInfo));
            _processChain.Verify(i => i.Append(_cmdProcessWrapper.Object));
            _processChain.Verify(i => i.Append(_dockerProcessWrapper.Object));
            _processChain.Verify(i => i.Append(_shProcessWrapper.Object));
        }

        private DockerWrapperService CreateInstance() =>
            new DockerWrapperService(
                _processChain.Object,
                () => _dockerProcessWrapper.Object,
                _cmdProcessWrapper.Object,
                _shProcessWrapper.Object);
    }
}