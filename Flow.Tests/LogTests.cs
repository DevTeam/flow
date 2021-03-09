namespace Flow.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using Moq;
    using Shouldly;
    using Xunit;

    public class LogTests
    {
        private readonly Mock<IColorfulStdOut> _stdOut;
        private readonly List<string> _text = new List<string>();

        public LogTests()
        {
            _stdOut = new Mock<IColorfulStdOut>();
            _stdOut.Setup(i => i.WriteLine(It.IsAny<Text[]>())).Callback<Text[]>(text => _text.AddRange(text.Select(i => i.ToString())));
        }

        [Theory]
        [InlineData(Verbosity.Quiet, "Default:abc", "")]
        [InlineData(Verbosity.Minimal, "Default:abc", "")]
        [InlineData(Verbosity.Normal, "Default:abc", "Trace:abc")]
        [InlineData(Verbosity.Detailed, "Default:abc", "Trace:abc")]
        [InlineData(Verbosity.Diagnostic, "Default:abc", "Trace:abc")]
        [InlineData(Verbosity.Normal, "Header:abc", "Header:abc")]
        [InlineData(Verbosity.Normal, "Header:abc Default:Xyz", "Header:abc|Trace:Xyz")]
        public void ShouldLogInfo(Verbosity verbosity, string messages, string expectedLog)
        {
            // Given
            var log = new Log<LogTests>(verbosity, _stdOut.Object);
            Enumerable<Text> text = messages;
            
            // When
            log.Info(() => text.ToArray());

            // Then
            string.Join("|", _text).ShouldBe(expectedLog);
        }

        [Theory]
        [InlineData(Verbosity.Quiet, "Default:abc", "Error:abc")]
        [InlineData(Verbosity.Minimal, "Default:abc", "Error:abc")]
        [InlineData(Verbosity.Normal, "Default:abc", "Error:abc")]
        [InlineData(Verbosity.Detailed, "Default:abc", "Trace:LogTests -> |Error:abc")]
        [InlineData(Verbosity.Diagnostic, "Default:abc", "Trace:LogTests -> |Error:abc")]
        [InlineData(Verbosity.Detailed, "Header:abc", "Trace:LogTests -> |Header:abc")]
        [InlineData(Verbosity.Detailed, "Header:abc Default:Xyz", "Trace:LogTests -> |Header:abc|Error:Xyz")]
        public void ShouldLogError(Verbosity verbosity, string messages, string expectedLog)
        {
            // Given
            var log = new Log<LogTests>(verbosity, _stdOut.Object);
            Enumerable<Text> text = messages;

            // When
            log.Error(text.ToArray());

            // Then
            string.Join("|", _text).ShouldBe(expectedLog);
        }

        [Theory]
        [InlineData(Verbosity.Quiet, "Default:abc", "")]
        [InlineData(Verbosity.Minimal, "Default:abc", "Warning:abc")]
        [InlineData(Verbosity.Normal, "Default:abc", "Warning:abc")]
        [InlineData(Verbosity.Detailed, "Default:abc", "Trace:LogTests -> |Warning:abc")]
        [InlineData(Verbosity.Diagnostic, "Default:abc", "Trace:LogTests -> |Warning:abc")]
        [InlineData(Verbosity.Detailed, "Header:abc", "Trace:LogTests -> |Header:abc")]
        [InlineData(Verbosity.Detailed, "Header:abc Default:Xyz", "Trace:LogTests -> |Header:abc|Warning:Xyz")]
        public void ShouldLogWarning(Verbosity verbosity, string messages, string expectedLog)
        {
            // Given
            var log = new Log<LogTests>(verbosity, _stdOut.Object);
            Enumerable<Text> text = messages;

            // When
            log.Warning(text.ToArray());

            // Then
            string.Join("|", _text).ShouldBe(expectedLog);
        }

        [Theory]
        [InlineData(Verbosity.Quiet, "Default:abc", "")]
        [InlineData(Verbosity.Minimal, "Default:abc", "")]
        [InlineData(Verbosity.Normal, "Default:abc", "")]
        [InlineData(Verbosity.Detailed, "Default:abc", "")]
        [InlineData(Verbosity.Diagnostic, "Default:abc", "Trace:LogTests -> |Trace:abc")]
        [InlineData(Verbosity.Diagnostic, "Header:abc", "Trace:LogTests -> |Header:abc")]
        [InlineData(Verbosity.Diagnostic, "Header:abc Default:Xyz", "Trace:LogTests -> |Header:abc|Trace:Xyz")]
        public void ShouldLogTrace(Verbosity verbosity, string messages, string expectedLog)
        {
            // Given
            var log = new Log<LogTests>(verbosity, _stdOut.Object);
            Enumerable<Text> text = messages;

            // When
            log.Trace(() => text.ToArray());

            // Then
            string.Join("|", _text).ShouldBe(expectedLog);
        }
    }
}
