namespace Flow.Tests
{
    using Core;
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
            var argumentsFactory = new ArgumentToStringConverter(' ', '"');

            // When
            var actualArgument = argumentsFactory.Convert(new CommandLineArgument(argument));

            // Then
            actualArgument.ShouldBe(expectedArgument);
        }
    }
}
