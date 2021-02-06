namespace Flow.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using IoC;
    using Services;
    using Shouldly;
    using Xunit;

    public class FromTextExtensionsTests
    {
        [Theory]
        [InlineData("a", 1, "a")]
        [InlineData("abc", 3, "abc")]
        [InlineData("AbC", 3, "AbC")]
        [InlineData("\"abc\"", 5, "abc")]
        [InlineData("", 0, "")]
        [InlineData(" ", 1, "")]
        [InlineData("   ", 3, "")]
        [InlineData("\"\"", 2, "")]
        [InlineData("\"  \"", 4, "  ")]
        [InlineData("'  '", 4, "  ")]
        [InlineData(" abc", 4, "abc")]
        [InlineData("abc ", 4, "abc")]
        [InlineData("   abc", 6, "abc")]
        [InlineData("abc   ", 4, "abc")]
        [InlineData("  abc  ", 6, "abc")]
        [InlineData("'\"abc\"'", 7, "\"abc\"")]
        [InlineData("\"'abc'\"", 7, "'abc'")]
        public void ShouldParse(string text, int expectedOffset, string expectedText)
        {
            // Given
            using (var enumerator = new TextEnumerator(text.GetEnumerator()))
            {
                // When
                var actualText = enumerator.Parse();

                // Then
                enumerator.Offset.ShouldBe(expectedOffset);
                actualText.ShouldBe(expectedText);
            }
        }

        [Theory]
        [InlineData("abc xyz", 7, "abc|xyz")]
        [InlineData("abc \"xy z\"", 10, "abc|xy z")]
        [InlineData("'ab c' xyz", 10, "ab c|xyz")]
        [InlineData("   abc  xyz  ", 13, "abc|xyz")]
        [InlineData("abc", 3, "abc")]
        [InlineData("  abc  ", 7, "abc")]
        [InlineData("", 0, null)]
        [InlineData("   ", 3, null)]
        public void ShouldParseEnumerable(string text, int expectedOffset, string expected)
        {
            // Given
            using (var enumerator = new TextEnumerator(text.GetEnumerator()))
            {
                var expectedResult = expected?.Split('|');

                // When
                var actual = enumerator.ParseEnumerable().ToArray();

                // Then
                enumerator.Offset.ShouldBe(expectedOffset);
                if (expectedResult == null)
                {
                    actual.Length.ShouldBe(0);
                }
                else
                {
                    actual.ShouldBe(expectedResult);
                }
            }
        }

        [Theory]
        [InlineData("var1=val1", 9, "var1", "val1")]
        [InlineData("\"va r1=val 1\"", 13, "va r1", "val 1")]
        [InlineData("'va r1=val 1'", 13, "va r1", "val 1")]
        [InlineData("'va \"r1=val 1'", 14, "va \"r1", "val 1")]
        public void ShouldParseTuple(string text, int expectedOffset, string expectedName, string expectedValue)
        {
            // Given
            using (var enumerator = new TextEnumerator(text.GetEnumerator()))
            {
                // When
                var (name, value) = enumerator.ParseTuple();

                // Then
                enumerator.Offset.ShouldBe(expectedOffset);
                name.ShouldBe(expectedName);
                value.ShouldBe(expectedValue);
            }
        }

        [Theory]
        [InlineData("abc", 3, "abc")]
        [InlineData("a", 1, "a")]
        [InlineData("\"abc\"", 5, "abc")]
        [InlineData("  \"a bc\"  ", 9, "a bc")]
        [InlineData("", 0, "")]
        [InlineData("   ", 3, "")]
        public void ShouldParseValue(string text, int expectedOffset, string expectedValue)
        {
            // Given
            using (var enumerator = new TextEnumerator(text.GetEnumerator()))
            {
                // When
                var actual = enumerator.Parse<Item>();

                // Then
                enumerator.Offset.ShouldBe(expectedOffset);
                actual.Value.ShouldBe(expectedValue);
            }
        }

        private readonly struct Item: IFromText<Item>
        {
            public readonly string Value;

            private Item([NotNull] string value) =>
                Value = value ?? throw new ArgumentNullException(nameof(value));

            public Item Parse(IEnumerator<char> text) => new Item(text.Parse());
        }

        private class TextEnumerator: IEnumerator<char>
        {
            private readonly IEnumerator<char> _baseEnumerator;
            private int _offset;

            public TextEnumerator(IEnumerator<char> baseEnumerator) =>
                _baseEnumerator = baseEnumerator;

            public int Offset => _offset;

            public char Current => _baseEnumerator.Current;

            object IEnumerator.Current => _baseEnumerator.Current;

            public bool MoveNext()
            {
                if (_baseEnumerator.MoveNext())
                {
                    _offset++;
                    return true;
                }

                return false;
            }

            public void Reset()
            {
                _baseEnumerator.Reset();
                _offset = 0;
            }

            public void Dispose() => _baseEnumerator.Dispose();
        }
    }
}
