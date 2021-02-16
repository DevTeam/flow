namespace Flow.Tests
{
    using System.Linq;
    using Core;
    using Moq;
    using Xunit;
    using OperatingSystem = OperatingSystem;

    public class DockerWrapperServiceTests
    {
        private readonly Mock<IProcessChain> _processChain;
        private readonly Mock<IProcessWrapper> _dockerProcessWrapper;
        private readonly Mock<IProcessWrapper> _cmdProcessWrapper;
        private readonly Mock<IProcessWrapper> _shProcessWrapper;

        public DockerWrapperServiceTests()
        {
            _processChain = new Mock<IProcessChain>();
            _dockerProcessWrapper = new Mock<IProcessWrapper>();
            _cmdProcessWrapper = new Mock<IProcessWrapper>();
            _shProcessWrapper = new Mock<IProcessWrapper>();
        }

        [Theory]
        [InlineData(OperatingSystem.Unix)]
        [InlineData(OperatingSystem.Windows)]
        [InlineData(OperatingSystem.Mac)]
        public void ShouldAddDockerWrapperToChain(OperatingSystem operatingSystem)
        {
            // Given
            DockerImage dockerImage = "mcr.microsoft.com/windows/servercore";
            var wrapperInfo = new DockerWrapperInfo(
                dockerImage,
                Enumerable.Empty<DockerVolume>(),
                Enumerable.Empty<CommandLineArgument>(),
                Enumerable.Empty<EnvironmentVariable>(),
                operatingSystem,
                true,
                DockerPullType.Missing);

            var wrapper = CreateInstance();

            // When
            using (wrapper.Using(wrapperInfo))
            {
                // Then
                _processChain.Verify(i => i.Append(_dockerProcessWrapper.Object));
                if (operatingSystem == OperatingSystem.Windows)
                {
                    _processChain.Verify(i => i.Append(_cmdProcessWrapper.Object));
                }
                else
                {
                    _processChain.Verify(i => i.Append(_shProcessWrapper.Object));
                }
            }
        }

        private DockerWrapperService CreateInstance() =>
            new DockerWrapperService(
                _processChain.Object,
                info => _dockerProcessWrapper.Object,
                _cmdProcessWrapper.Object,
                _shProcessWrapper.Object);
    }
}