namespace Flow.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using Shouldly;
    using Xunit;

    public class CommandLineParserTests
    {
        public static IEnumerable<object[]> TestData => new List<object[]>
        {
            new object[]
            {
                new [] { "--Name", "Value" },
                new [] { new KeyValueArgument("Name", "Value") }
            },
            new object[]
            {
                new [] { "--" },
                new [] { new KeyValueArgument("", "--") }
            },
            new object[]
            {
                new [] { "Abc"},
                new [] { new KeyValueArgument("", "Abc") }
            },
            new object[]
            {
                new [] { "Abc", "--Name", "Value", "--", "--Name2", "Value2" },
                new []
                {
                    new KeyValueArgument("", "Abc"),
                    new KeyValueArgument("Name", "Value"),
                    new KeyValueArgument("", "--"),
                    new KeyValueArgument("Name2", "Value2")
                }
            },
        };

        [Theory]
        [MemberData(nameof(TestData))]
        internal void ShouldParse(string[] args, KeyValueArgument[] expectedArgs)
        {
            // Given
            var parser = new CommandLineParser();

            // When
            var actualArgs = parser.Parse(args).ToArray();

            // Then
            actualArgs.ShouldBe(expectedArgs);
        }
    }
}
