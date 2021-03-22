namespace Flow.Tests
{
    using Core;
    using Shouldly;
    using Xunit;

    // ReSharper disable once InconsistentNaming
    public class MSBuildParameterToStringConverterTests
    {
        [Theory]
        [InlineData("name", "name")]
        [InlineData("Name", "Name")]

        // special
        [InlineData("_Name", "_Name")]
        [InlineData("Na_me", "Na_me")]
        [InlineData("Na_me_", "Na_me_")]
        [InlineData("Na-me-", "Na-me-")]
        [InlineData("Na_m88e-9", "Na_m88e-9")]
        
        // first symbol
        [InlineData("1name", "_name")]
        [InlineData("=name", "_name")]
        [InlineData("-name", "_name")]
        [InlineData("@name", "_name")]
        [InlineData("+name", "_name")]
        [InlineData("#name", "_name")]
        [InlineData("$name", "_name")]
        
        // other symbols
        [InlineData("name#", "name_")]
        [InlineData("name.aa", "name_aa")]
        [InlineData("name.Aa", "name_Aa")]
        [InlineData("name Aa", "name_Aa")]
        [InlineData("name^aa", "name_aa")]
        [InlineData("name&aa", "name_aa")]
        [InlineData("name*aa", "name_aa")]
        [InlineData("name=aa", "name_aa")]
        [InlineData("name!", "name_")]
        [InlineData("name>", "name_")]
        [InlineData("name<", "name_")]
        [InlineData("name?", "name_")]
        public void ShouldNormalizeName(string name, string expectedName)
        {
            // Given
            var converter = new MSBuildParameterToStringConverter();

            // When
            var actualName = converter.NormalizeName(name);

            // Then
            actualName.ShouldBe(expectedName);
        }

        [Theory]
        [InlineData("!@#$%^&*()_+~1234-=/;'][{}\":<>,.?/??~`", "\"%21%40%23%24%%5E%26%2A%28%29%5F%2B%7E1234%2D%3D%2F;%27%5D%5B%7B%7D%22%3A%3C%3E%2C%2E%3F%2F%3F%3F%7E%60\"", false)]
        [InlineData("value 123", "value%20123", false)]
        [InlineData("value \" 123", "value%20%22%20123", false)]
        [InlineData("value \\ 123", "value%20%5C%20123", false)]
        [InlineData("value \"\" 123", "value%20%22%22%20123", false)]
        [InlineData("value \" \" 123", "value%20%22%20%22%20123", false)]
        [InlineData("value1 \n value2", "value1%20%0A%20value2", false)]
        [InlineData("value1 \r value2", "value1%20%0D%20value2", false)]
        [InlineData("value1 \t value2", "value1%20%09%20value2", false)]
        [InlineData("value1 \b value2", "value1%20%08%20value2", false)]

        // should not escape `;` for response files and should wrap a parameter by double quotes in this case (https://github.com/JetBrains/teamcity-dotnet-plugin/issues/144)
        [InlineData("Value;123", "\"Value;123\"", false)]

        // should escape `;` for command line parameters https://youtrack.jetbrains.com/issue/TW-64835
        [InlineData("Value;123", "Value%3B123", true)]
        public void ShouldNormalizeValue(string value, string expectedValue, bool isCommandLineParameter)
        {
            // Given
            var converter = new MSBuildParameterToStringConverter();

            // When
            var actualValue = converter.NormalizeValue(value, isCommandLineParameter);

            // Then
            actualValue.ShouldBe(expectedValue);
        }

        [Fact]
        public void ShouldConvert()
        {
            // Given
            var converter = new MSBuildParameterToStringConverter();

            // When
            var actualValue = converter.Convert(new MSBuildParameter("Prop", "Val"));

            // Then
            actualValue.ShouldBe("/p:Prop=Val");
        }
    }
}