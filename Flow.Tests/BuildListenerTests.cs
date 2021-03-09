namespace Flow.Tests
{
    using System.Diagnostics;
    using Core;
    using Moq;
    using Shouldly;
    using Xunit;

    public class BuildListenerTests
    {
        private readonly Mock<ILog<BuildListener>> _log;
        private readonly Mock<IColorfulStdOut> _stdOut;
        private readonly Mock<IStdErr> _stdErr;
        private readonly Mock<IMessageProcessor> _messageProcessor;
        
        public BuildListenerTests()
        {
            _log = new Mock<ILog<BuildListener>>();
            _stdOut = new Mock<IColorfulStdOut>();
            _stdErr = new Mock<IStdErr>();
            _messageProcessor = new Mock<IMessageProcessor>();
        }

        [Fact]
        public void ShouldLogProcessInfoOnStart()
        {
            // Given
            var listener = CreateInstance();

            // When
            listener.OnStart(new ProcessStartInfo("abc.exe", "args") { WorkingDirectory = "wd"});

            // Then
            _log.VerifyInfo(new Text("Starting: abc.exe args", Color.Header));
            _log.VerifyInfo(new Text($"in directory: {System.IO.Path.GetFullPath("wd")}", Color.Header));
        }

        [Fact]
        public void ShouldLogExitCodeWhenZeroOnExit()
        {
            // Given
            var listener = CreateInstance();

            // When
            listener.OnExitCode(new ExitCode(0));

            // Then
            _log.VerifyInfo(new Text("Exit code: ", Color.Header), new Text("0", Color.Success));
        }

        [Fact]
        public void ShouldLogExitCodeWhenNotZeroOnExit()
        {
            // Given
            var listener = CreateInstance();

            // When
            listener.OnExitCode(new ExitCode(99));

            // Then
            _log.VerifyInfo(new Text("Exit code: ", Color.Header), new Text("99", Color.Error));
        }

        [Fact]
        public void ShouldLogProcessInfoOnStdErr()
        {
            // Given
            var listener = CreateInstance();

            // When
            listener.OnStdErr("Some error");

            // Then
            _stdErr.Verify(i => i.WriteLine("Some error"));
        }

        [Fact]
        public void ShouldLogProcessInfoOnStdOutAndHasNoAnyMessages()
        {
            // Given
            var listener = CreateInstance();
            _messageProcessor.Setup(i => i.ProcessServiceMessages("Some text", listener)).Returns(false);

            // When
            listener.OnStdOut("Some text");

            // Then
            _stdOut.Verify(i => i.WriteLine(new Text("Some text", Color.Default)));
        }

        [Fact]
        public void ShouldProcessMessage()
        {
            // Given
            var listener = CreateInstance();
            _messageProcessor.Setup(i => i.ProcessServiceMessages("Some text", listener)).Returns(true);

            // When
            listener.OnStdOut("Some text");

            // Then
            _messageProcessor.Verify(i => i.ProcessServiceMessages("Some text", listener));
            _stdOut.Verify(i => i.WriteLine(It.IsAny<Text[]>()), Times.Never);
        }

        [Fact]
        public void ShouldProvideBuildResultWhenFailed()
        {
            // Given
            var listener = CreateInstance();
            
            // When
            listener.Visit(new BuildError("error 1"));
            listener.Visit(new BuildError("error 2"));
            listener.Visit(new BuildWarning("warning 1"));
            listener.Visit(new BuildWarning("warning 2"));
            listener.OnExitCode(99);
            var result = listener.Result;

            // Then
            result.Success.ShouldBeFalse();
            result.Errors.ShouldBe(new[] { new BuildError("error 1"), new BuildError("error 2") });
            result.Warnings.ShouldBe(new[] { new BuildWarning("warning 1"), new BuildWarning("warning 2") });
        }

        [Fact]
        public void ShouldProvideBuildResultWhenFailedAndHasErrors()
        {
            // Given
            var listener = CreateInstance();

            // When
            listener.Visit(new BuildError("error 1"));
            listener.Visit(new BuildError("error 2"));
            listener.Visit(new BuildWarning("warning 1"));
            listener.Visit(new BuildWarning("warning 2"));
            listener.OnExitCode(0);
            var result = listener.Result;

            // Then
            result.Success.ShouldBeFalse();
            result.Errors.ShouldBe(new[] { new BuildError("error 1"), new BuildError("error 2") });
            result.Warnings.ShouldBe(new[] { new BuildWarning("warning 1"), new BuildWarning("warning 2") });
        }

        [Fact]
        public void ShouldProvideBuildResultWhenFailedAndHasNoErrors()
        {
            // Given
            var listener = CreateInstance();

            // When
            listener.Visit(new BuildWarning("warning 1"));
            listener.Visit(new BuildWarning("warning 2"));
            listener.OnExitCode(99);
            var result = listener.Result;

            // Then
            result.Success.ShouldBeFalse();
            result.Errors.ShouldBeEmpty();
            result.Warnings.ShouldBe(new[] { new BuildWarning("warning 1"), new BuildWarning("warning 2") });
        }

        [Fact]
        public void ShouldProvideBuildResultWhenSuccess()
        {
            // Given
            var listener = CreateInstance();

            // When
            listener.Visit(new BuildWarning("warning 1"));
            listener.Visit(new BuildWarning("warning 2"));
            listener.OnExitCode(0);
            var result = listener.Result;

            // Then
            result.Success.ShouldBeTrue();
            result.Errors.ShouldBeEmpty();
            result.Warnings.ShouldBe(new[] { new BuildWarning("warning 1"), new BuildWarning("warning 2") });
        }

        private BuildListener CreateInstance() => new BuildListener(
            _log.Object,
            _stdOut.Object,
            _stdErr.Object,
            _messageProcessor.Object);
    }
}
