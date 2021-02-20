namespace Flow.Tests
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using Core;
    using Moq;
    using Xunit;

    public class ProcessFactoryTests
    {
        private readonly Mock<Func<Process, IProcess>> _processFactory;
        private readonly Path _workingDirectory = "wd";
        private readonly Mock<IConverter<CommandLineArgument, string>> _argumentToStringConverter;

        public ProcessFactoryTests()
        {
            var process = new Mock<IProcess>();

            _processFactory = new Mock<Func<Process, IProcess>>();
            _processFactory.Setup(i => i(It.IsAny<Process>())).Returns(process.Object);

            _argumentToStringConverter = new Mock<IConverter<CommandLineArgument, string>>();
            _argumentToStringConverter.Setup(i => i.Convert(It.IsAny<CommandLineArgument>())).Returns<CommandLineArgument>(arg => arg.Value);
        }

        [Fact]
        public void ShouldCreateProcess()
        {
            // Given
            var info = ProcessInfoExtensions.Create();
            var factory = CreateInstance();

            // When
            factory.Create(info);

            // Then
            _processFactory.Verify(func => func(It.Is<Process>(
                process =>
                    process.StartInfo.FileName == info.Executable.Value
                    && process.StartInfo.WorkingDirectory == info.WorkingDirectory.Value
                    && process.StartInfo.Arguments == info.Arguments.ToString()
                    && info.Variables.All(i => process.StartInfo.Environment[i.Name] == i.Value)
                    && process.StartInfo.EnvironmentVariables[ProcessFactory.FlowVersionEnvName] == "1.0.0.0"
            )));
        }

        private ProcessFactory CreateInstance() => 
            new ProcessFactory(
                _processFactory.Object,
                _workingDirectory,
                "1.0.0.0",
                ' ',
                _argumentToStringConverter.Object);
    }
}
