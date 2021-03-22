namespace Flow.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using Moq;
    using Shouldly;
    using Xunit;

    public class CmdProcessWrapperTests
    {
        private readonly Path _tempFilePath = new Path("Tmp");
        private readonly char _argumentsSeparator = ' ';
        private readonly char _argumentsQuote = '"';
        private readonly Mock<IFileSystem> _fileSystem = new Mock<IFileSystem>();
        private readonly Mock<IConverter<IEnumerable<CommandLineArgument>, string>> _argumentsToStringConverter = new Mock<IConverter<IEnumerable<CommandLineArgument>, string>>();

        [Fact]
        public void ShouldWrap()
        {
            // Given
            var wrapper = CreateInstance();
            var cmdPath = new Path(_tempFilePath.Value + ".cmd");
            _argumentsToStringConverter
                .Setup(i => i.Convert(It.IsAny<IEnumerable<CommandLineArgument>>()))
                .Returns<IEnumerable<CommandLineArgument>>(args => 
                    string.Join(" ", args.Select(j => $"\"{j.Value}\"")));

            // When
            var actualProcessInfo = wrapper.Wrap(
                new ProcessInfo(
                    "abc.exe",
                    "wd",
                    new[] { new CommandLineArgument("Arg1") },
                    new []{ new EnvironmentVariable("Env1", "Var1") })
            );

            // Then
            actualProcessInfo.Executable.ShouldBe("cmd.exe");
            actualProcessInfo.WorkingDirectory.ShouldBe("wd");
            actualProcessInfo.Arguments.ShouldBe(new CommandLineArgument[]{ "/C", cmdPath.Value });
            actualProcessInfo.Variables.ShouldBeEmpty();
            _fileSystem.Verify(
                i => i.WriteLines(
                    cmdPath,
                    It.Is<IEnumerable<string>>(content => content.SequenceEqual(
                        new []
                        {
                            "@echo off",
                            "pushd \"wd\"",
                            "set \"Env1=Var1\"",
                            "\"abc.exe\" \"Arg1\"",
                            "set exitCode=%errorlevel%",
                            "popd",
                            "exit /b %exitCode%"
                        }))
                    )
                );
        }

        private CmdProcessWrapper CreateInstance() =>
            new CmdProcessWrapper(
                _tempFilePath,
                _argumentsSeparator,
                _argumentsQuote,
                _fileSystem.Object,
                _argumentsToStringConverter.Object);
    }
}
