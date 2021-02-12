namespace Flow.Tests
{
    using Core;
    using Moq;
    using Xunit;

    public class DockerProcessWrapperTests
    {
        private readonly Path _tempPath = "tmp";
        private readonly Mock<IEnvironment> _environment;
        private readonly Mock<IProcessWrapper> _processWrapper;

        public DockerProcessWrapperTests()
        {
            _environment = new Mock<IEnvironment>();
            _processWrapper = new Mock<IProcessWrapper>();
        }

        [Fact]
        public void ShouldWrap()
        {
            // Given
            var wrapper = CreateInstance();
            DockerImage dockerImage = "mcr.microsoft.com/windows/servercore";
            var wrapperInfo = new DockerWrapperInfo(dockerImage);
            wrapper.Initialize(wrapperInfo);
            var processInfo = ProcessInfoExtensions.Create();

            // When
            var actualProcessInfo = wrapper.Wrap(processInfo);

            // Then
        }

        private DockerProcessWrapper CreateInstance() => 
            new DockerProcessWrapper(
                _tempPath,
                _environment.Object,
                _processWrapper.Object);
    }
}
