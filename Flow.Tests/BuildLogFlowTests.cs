namespace Flow.Tests
{
    using System;
    using System.Linq;
    using Core;
    using JetBrains.TeamCity.ServiceMessages.Write;
    using Moq;
    using Shouldly;
    using Xunit;

    public class BuildLogFlowTests
    {
        private readonly Mock<IStdOut> _stdOut;
        private readonly Mock<Func<int, IBuildLogFlow>> _flowFactory;
        private readonly Mock<ILog<BuildLogFlow>> _log;

        public BuildLogFlowTests()
        {
            _log = new Mock<ILog<BuildLogFlow>>();
            _stdOut = new Mock<IStdOut>();
            _flowFactory = new Mock<Func<int, IBuildLogFlow>>();
        }

        [Fact]
        public void ShouldCreateChildFlow()
        {
            // Given
            var childFlow = new Mock<IBuildLogFlow>();
            var flow = CreateInstance(2);
            _flowFactory.Setup(i => i(2)).Returns(childFlow.Object);

            // When
            var actualChildFlow = flow.CreateChild();

            // Then
            actualChildFlow.ShouldBe(childFlow.Object);
        }

        [Fact]
        public void ShouldProcessBlockOpened()
        {
            // Given
            var childFlow = new Mock<IBuildLogFlow>();
            var flow = CreateInstance(2);
            _flowFactory.Setup(i => i(3)).Returns(childFlow.Object);
            
            var blockOpenedMessage = new ServiceMessage("blocKopened")
            {
                {"name", "bl"}
            };

            // When
            flow.ProcessMessage(
                Mock.Of<IMessageProcessor>(),
                Mock.Of<IBuildVisitor>(),
                blockOpenedMessage);

            flow.CreateChild();

            // Then
            _stdOut.Verify(i => i.WriteLine("bl[0;37;40m"));
        }

        [Fact]
        public void ShouldProcessBlockClosed()
        {
            // Given
            var childFlow = new Mock<IBuildLogFlow>();
            var flow = CreateInstance(2);
            _flowFactory.Setup(i => i(2)).Returns(childFlow.Object);

            var blockOpenedMessage = new ServiceMessage("blocKopened");
            var blockClosedMessage = new ServiceMessage("blocKclosed");

            // When
            flow.ProcessMessage(
                Mock.Of<IMessageProcessor>(),
                Mock.Of<IBuildVisitor>(),
                blockOpenedMessage);

            flow.ProcessMessage(
                Mock.Of<IMessageProcessor>(),
                Mock.Of<IBuildVisitor>(),
                blockClosedMessage);

            flow.CreateChild();

            // Then
        }

        [Theory]
        [InlineData("Abc", "Warning", null, false, "", "Abc", "Abc", "")]
        [InlineData("Abc", "Error", null, false, "Abc", "", "Abc", "")]
        [InlineData("Abc", "Normal", null, false, "", "", "Abc", "")]
        [InlineData("Abc", "Normal", "TC:parseServiceMessagesInside", false, "", "", "Abc", "Abc")]
        [InlineData("Abc", "Normal", "TC:parseServiceMessagesInside", true, "", "", "", "Abc")]

        public void ShouldProcessMessage(
            string text,
            string status,
            string parseInternalTag,
            bool processedInternal,
            string expectedErrors,
            string expectedWarnings,
            string expectedLines,
            string expectedParseInternalLines)
        {
            // Given
            var childFlow = new Mock<IBuildLogFlow>();
            var flow = CreateInstance(2);
            _flowFactory.Setup(i => i(It.IsAny<int>())).Returns(childFlow.Object);

            var blockOpenedMessage = new ServiceMessage("meSsage")
            {
                { "text", text },
                { "status", status },
                { "tc:tags", parseInternalTag }
            };

            var processor = new Mock<IMessageProcessor>();
            var visitor = new Mock<IBuildVisitor>();

            foreach (var line in expectedParseInternalLines.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
            {
                processor.Setup(i => i.ProcessServiceMessages(line, visitor.Object)).Returns(processedInternal);
            }

            // When
            flow.ProcessMessage(
                processor.Object,
                visitor.Object,
                blockOpenedMessage);

            flow.CreateChild();

            // Then
            foreach (var buildError in expectedErrors.Split(new [] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(i => new BuildError(i)))
            {
                visitor.Verify(i => i.Visit(buildError));
            }

            foreach (var buildWarning in expectedWarnings.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(i => new BuildWarning(i)))
            {
                visitor.Verify(i => i.Visit(buildWarning));
            }

            foreach (var line in expectedLines.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
            {
                _stdOut.Verify(i => i.WriteLine(line + "[0;37;40m"));
            }

            foreach (var line in expectedParseInternalLines.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
            {
                processor.Verify(i => i.ProcessServiceMessages(line, visitor.Object));
            }
        }

        private BuildLogFlow CreateInstance(int tabs = 0) =>
            new BuildLogFlow(
                _log.Object,
                _stdOut.Object,
                _flowFactory.Object,
                tabs);
    }
}
