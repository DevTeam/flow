namespace Flow.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Core;
    using JetBrains.TeamCity.ServiceMessages;
    using JetBrains.TeamCity.ServiceMessages.Read;
    using JetBrains.TeamCity.ServiceMessages.Write;
    using Moq;
    using Shouldly;
    using Xunit;

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public class MessageProcessorTests
    {
        private readonly Mock<ILog<MessageProcessor>> _log;
        private readonly Mock<IServiceMessageParser> _serviceMessageParser;
        private readonly Mock<Func<IBuildLogFlow>> _flowFactory;
        private readonly Mock<IBuildVisitor> _buildVisitor;
        private readonly  Mock<IServiceMessageFormatter> _messageFormatter;

        public MessageProcessorTests()
        {
            _log = new Mock<ILog<MessageProcessor>>();
            _messageFormatter = new Mock<IServiceMessageFormatter>();
            _serviceMessageParser = new Mock<IServiceMessageParser>();
            _flowFactory = new Mock<Func<IBuildLogFlow>>();
            _buildVisitor = new Mock<IBuildVisitor>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void ShouldSkipEmptyLines(string line)
        {
            // Given
            var processor = CreateInstance();

            // When
            var result = processor.ProcessServiceMessages(line, _buildVisitor.Object);

            // Then
            result.ShouldBeFalse();
            _buildVisitor.Verify(i => i.Visit(It.IsAny<BuildError>()), Times.Never);
            _buildVisitor.Verify(i => i.Visit(It.IsAny<BuildWarning>()), Times.Never);
        }

        [Fact]
        public void ShouldProcessMessages()
        {
            // Given
            var processor = CreateInstance();

            var message1 = new ServiceMessage("message")
            {
                {"flowId", "myFlowId1"}
            };

            var message2 = new ServiceMessage("test")
            {
                {"flowId", "myFlowId2"}
            };

            _serviceMessageParser
                .Setup(i => i.ParseServiceMessages("messages"))
                .Returns(new List<IServiceMessage>
                {
                    message1,
                    message2
                });

            var flow1 = new Mock<IBuildLogFlow>();
            flow1.Setup(i => i.ProcessMessage(processor, _buildVisitor.Object, message1)).Returns(false);

            var flow2 = new Mock<IBuildLogFlow>();
            flow2.Setup(i => i.ProcessMessage(processor, _buildVisitor.Object, message2)).Returns(true);

            var order = 0;
            _flowFactory.When(() => order++ == 1).Setup(i => i()).Returns(flow1.Object);
            _flowFactory.When(() => order++ == 2).Setup(i => i()).Returns(flow2.Object);

            // When
            var result = processor.ProcessServiceMessages("messages", _buildVisitor.Object);

            // Then
            result.ShouldBeTrue();
            processor.Flows.Count.ShouldBe(2);
            processor.Flows["myFlowId1"].ShouldBe(flow1.Object);
            processor.Flows["myFlowId2"].ShouldBe(flow2.Object);
        }

        [Fact]
        public void ShouldNotProcessMessagesWhenNotProcessed()
        {
            // Given
            var processor = CreateInstance();

            var message = new ServiceMessage("message")
            {
                {"flowId", "myFlowId"}
            };

            _serviceMessageParser
                .Setup(i => i.ParseServiceMessages("messages"))
                .Returns(new List<IServiceMessage>
                {
                    message,
                });

            var flow = new Mock<IBuildLogFlow>();
            flow.Setup(i => i.ProcessMessage(processor, _buildVisitor.Object, message)).Returns(false);

            _flowFactory.Setup(i => i()).Returns(flow.Object);
            
            // When
            var result = processor.ProcessServiceMessages("messages", _buildVisitor.Object);

            // Then
            result.ShouldBeFalse();
            processor.Flows.Count.ShouldBe(1);
            processor.Flows["myFlowId"].ShouldBe(flow.Object);
        }

        [Fact]
        public void ShouldNotProcessMessagesWhenNotParsed()
        {
            // Given
            var processor = CreateInstance();

            _serviceMessageParser
                .Setup(i => i.ParseServiceMessages("messages"))
                .Returns(new List<IServiceMessage>());

            // When
            var result = processor.ProcessServiceMessages("messages", _buildVisitor.Object);

            // Then
            result.ShouldBeFalse();
            processor.Flows.Count.ShouldBe(0);
        }

        [Fact]
        public void ShouldProcessFlowStarted()
        {
            // Given
            var processor = CreateInstance();

            var flowStarted = new ServiceMessage("flowstarteD")
            {
                {"parent", "myParentId"}, {"flowId", "myFlowId"}
            };

            _serviceMessageParser
                .Setup(i => i.ParseServiceMessages("messages"))
                .Returns(new List<IServiceMessage>
                {
                    flowStarted
                });

            var childFlow = new Mock<IBuildLogFlow>();

            var parentFlow = new Mock<IBuildLogFlow>();
            parentFlow.Setup(i => i.CreateChild()).Returns(childFlow.Object);
            _flowFactory.Setup(i => i()).Returns(parentFlow.Object);

            // When
            var result = processor.ProcessServiceMessages("messages", _buildVisitor.Object);

            // Then
            result.ShouldBeTrue();
            processor.Flows.Count.ShouldBe(2);
            parentFlow.Verify(i => i.CreateChild());
            processor.Flows["myParentId"].ShouldBe(parentFlow.Object);
            processor.Flows["myFlowId"].ShouldBe(childFlow.Object);
        }

        [Fact]
        public void ShouldProcessFlowFinished()
        {
            // Given
            var processor = CreateInstance();

            var flowStarted = new ServiceMessage("flowstarteD")
            {
                {"parent", "myParentId"}, {"flowId", "myFlowId"}
            };

            var flowFinished = new ServiceMessage("flowFiniShed")
            {
                {"flowId", "myFlowId"}
            };

            _serviceMessageParser
                .Setup(i => i.ParseServiceMessages("messages1"))
                .Returns(new List<IServiceMessage>
                {
                    flowStarted,
                });

            _serviceMessageParser
                .Setup(i => i.ParseServiceMessages("messages2"))
                .Returns(new List<IServiceMessage>
                {
                    flowFinished,
                });

            var childFlow = new Mock<IBuildLogFlow>();

            var parentFlow = new Mock<IBuildLogFlow>();
            parentFlow.Setup(i => i.CreateChild()).Returns(childFlow.Object);
            _flowFactory.Setup(i => i()).Returns(parentFlow.Object);

            // When
            var result = 
                processor.ProcessServiceMessages("messages1", _buildVisitor.Object)
                && processor.ProcessServiceMessages("messages2", _buildVisitor.Object);

            // Then
            result.ShouldBeTrue();
            processor.Flows.Count.ShouldBe(1);
            processor.Flows["myParentId"].ShouldBe(parentFlow.Object);
        }

        private MessageProcessor CreateInstance() =>
            new MessageProcessor(
                _log.Object,
                _messageFormatter.Object,
                _serviceMessageParser.Object,
                _flowFactory.Object);
    }
}
