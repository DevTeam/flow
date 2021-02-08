namespace Flow.Tests
{
    using Core;
    using Moq;
    using Shouldly;
    using Xunit;

    public class ArgumentToStringConverterTests
    {
        [Theory]
        [InlineData("Abc", "Abc")]
        [InlineData("a bc", "\"a bc\"")]
        [InlineData("\"a bc\"", "\"a bc\"")]
        [InlineData("\"abc\"", "abc")]
        [InlineData("ab\"\"c", "ab\"\"c")]
        public void ShouldConvert(string argument, string expectedArgument)
        {
            // Given
            var env = new Mock<IEnvironment>();
            env.SetupGet(i => i.CommandLineArgumentsSeparator).Returns(' ');
            env.SetupGet(i => i.CommandLineArgumentsQuote).Returns('"');
            var argumentsFactory = new ArgumentToStringConverter(env.Object);

            // When
            var actualArgument = argumentsFactory.Convert(new CommandLineArgument(argument));

            // Then
            actualArgument.ShouldBe(expectedArgument);
        }
    }
}
