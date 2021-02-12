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
            Path executable = "cmd.exe";
            Path workingDirectory = "wd";
            Enumerable<CommandLineArgument> arguments = "/? abc";
            Enumerable<EnvironmentVariable> variables = "var1=va1 var2=va2";
            ExitCode exitCode = 33;

            var processListener = new Mock<IProcessListener>();

            var process = new Mock<IProcess>();
            process.Setup(i => i.Run(processListener.Object)).Returns(exitCode);

            var processFactory = new Mock<IProcessFactory>();
            processFactory.Setup(i => i.Create(new ProcessInfo(executable, workingDirectory, arguments, variables))).Returns(process.Object);

            var commandLine = new CommandLineService(processFactory.Object, processListener.Object);
            
            // When
            var actualExitCode = commandLine.Execute(executable, workingDirectory, arguments, variables);

            // Then
            process.Verify(i => i.Run(processListener.Object));
            actualExitCode.ShouldBe(exitCode);
        }
    }
}
