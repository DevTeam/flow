namespace Flow.Tests
{
    using Core;
    using Moq;
    using Shouldly;
    using Xunit;

    public class CommandLineServiceTests
    {
        [Fact]
        public void ShouldExecute()
        {
            // Given
            var processInfo = ProcessInfoExtensions.Create();
            ExitCode exitCode = 33;

            var processListener = new Mock<IProcessListener>();

            var process = new Mock<IProcess>();
            process.Setup(i => i.Run(processListener.Object)).Returns(exitCode);

            var processFactory = new Mock<IProcessFactory>();
            processFactory.Setup(i => i.Create(processInfo)).Returns(process.Object);

            var commandLine = new CommandLineService(processFactory.Object, processListener.Object);
            
            // When
            var actualExitCode = commandLine.Execute(processInfo);

            // Then
            process.Verify(i => i.Run(processListener.Object));
            actualExitCode.ShouldBe(exitCode);
        }
    }
}
