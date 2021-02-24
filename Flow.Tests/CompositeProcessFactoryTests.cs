namespace Flow.Tests
{
    using Core;
    using Moq;
    using Xunit;

    public class CompositeProcessFactoryTests
    {
        private readonly Mock<IProcessFactory> _processFactory;
        private readonly Mock<IProcessWrapper> _processWrapper;
        private readonly Mock<IProcess> _process;

        public CompositeProcessFactoryTests()
        {
            _process = new Mock<IProcess>();
            _process.Setup(i => i.Run(It.IsAny<IProcessListener>())).Returns(99);

            _processFactory = new Mock<IProcessFactory>();

            _processWrapper = new Mock<IProcessWrapper>();
        }

        [Fact]
        public void ShouldAppendWrapper()
        {
            // Given
            var originalProcessInfo = ProcessInfoExtensions.Create();
            _processFactory.Setup(i => i.Create(originalProcessInfo)).Returns(_process.Object);

            var processInfo = ProcessInfoExtensions.Create("run.cmd");
            _processWrapper.Setup(i => i.Wrap(originalProcessInfo)).Returns(processInfo);

            var factory = CreateInstance();

            // When
            factory.Append(_processWrapper.Object);
            factory.Create(originalProcessInfo);

            // Then
            _processFactory.Verify(i => i.Create(processInfo));
        }

        [Fact]
        public void ShouldRemoveWrapper()
        {
            // Given
            var originalProcessInfo = ProcessInfoExtensions.Create();
            _processFactory.Setup(i => i.Create(originalProcessInfo)).Returns(_process.Object);

            var processInfo = ProcessInfoExtensions.Create("run.cmd");
            _processWrapper.Setup(i => i.Wrap(originalProcessInfo)).Returns(processInfo);

            var factory = CreateInstance();
            var wrapperToken = factory.Append(_processWrapper.Object);

            // When
            wrapperToken.Dispose();
            factory.Create(originalProcessInfo);

            // Then
            _processFactory.Verify(i => i.Create(originalProcessInfo));
        }

        private CurrentProcessFactory CreateInstance() =>
            new CurrentProcessFactory(_processFactory.Object);
    }
}
