namespace Flow.Tests
{
    using Core;
    using Xunit;

    public class DockerProcessWrapperTests
    {
        private readonly Path _tempPath = "tmp";

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
            new DockerProcessWrapper(_tempPath);
    }
}
