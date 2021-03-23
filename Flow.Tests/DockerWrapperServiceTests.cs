namespace Flow.Tests
{
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Core;
    using Moq;
    using Xunit;
    using OperatingSystem = OperatingSystem;

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public class DockerWrapperServiceTests
    {
        private readonly Mock<IChain<IProcessWrapper>> _processChain;
        private readonly Mock<IChain<IEnvironment>> _envChain;
        private readonly Mock<IProcessWrapper> _dockerProcessWrapper;
        private readonly Mock<IProcessWrapper> _cmdProcessWrapper;
        private readonly Mock<IProcessWrapper> _shProcessWrapper;
        private readonly Mock<IVirtualEnvironment> _virtualEnvironment;

        public DockerWrapperServiceTests()
        {
            _processChain = new Mock<IChain<IProcessWrapper>>();
            _envChain = new Mock<IChain<IEnvironment>>();
            _virtualEnvironment = new Mock<IVirtualEnvironment>();
            _dockerProcessWrapper = new Mock<IProcessWrapper>();
            _cmdProcessWrapper = new Mock<IProcessWrapper>();
            _shProcessWrapper = new Mock<IProcessWrapper>();

            _virtualEnvironment.Setup(i => i.Set(It.IsAny<OperatingSystem>())).Returns(_virtualEnvironment.Object);
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
                _virtualEnvironment.Verify(i => i.Set(operatingSystem));
                _envChain.Verify(i => i.Append(_virtualEnvironment.Object));
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
                _envChain.Object,
                _virtualEnvironment.Object,
                info => _dockerProcessWrapper.Object,
                _cmdProcessWrapper.Object,
                _shProcessWrapper.Object);
    }
}