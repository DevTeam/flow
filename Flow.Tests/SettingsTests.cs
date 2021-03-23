namespace Flow.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Core;
    using Moq;
    using Shouldly;
    using Xunit;

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public class SettingsTests
    {
        public static IEnumerable<object[]> TestData => new List<object[]>
        {
            new object[]
            {
                new KeyValueArgument[] { },
                new Settings(Mock.Of<ICommandLineParser>(), Mock.Of<Func<ILog<Settings>>>())
                {
                     ActivityName = "Default",
                     Timeout = TimeSpan.MaxValue,
                     Verbosity = Verbosity.Normal
                },
                false
            },
            new object[]
            {
                new[]
                {
                    new KeyValueArgument("", "Abc")
                },
                new Settings(Mock.Of<ICommandLineParser>(), Mock.Of<Func<ILog<Settings>>>())
                {
                    ActivityName = "Abc",
                    Timeout = TimeSpan.MaxValue,
                    Verbosity = Verbosity.Normal
                },
                false
            },
            new object[]
            {
                new[]
                {
                    new KeyValueArgument("", "Abc"),
                    new KeyValueArgument("verbosity", "Detailed"),
                    new KeyValueArgument("timeout", "05:03:04")
                },
                new Settings(Mock.Of<ICommandLineParser>(), Mock.Of<Func<ILog<Settings>>>())
                {
                    ActivityName = "Abc",
                    Timeout = new TimeSpan(5, 3, 4),
                    Verbosity = Verbosity.Detailed
                },
                false
            },
            new object[]
            {
                new[]
                {
                    new KeyValueArgument("", "Abc"),
                    new KeyValueArgument("verBosity", "DetaileD"),
                    new KeyValueArgument("tiMeout", "05:03:04"),
                    new KeyValueArgument("", "--"),
                    new KeyValueArgument("Var1", "Val1"),
                    new KeyValueArgument("Var2", "Val2")
                },
                new Settings(Mock.Of<ICommandLineParser>(), Mock.Of<Func<ILog<Settings>>>())
                {
                    ActivityName = "Abc",
                    Timeout = new TimeSpan(5, 3, 4),
                    Verbosity = Verbosity.Detailed,
                    Inputs = { { "Var1", "Val1" }, { "Var2", "Val2" } }
                },
                false
            },

            // Failed
            new object[]
            {
                new[]
                {
                    new KeyValueArgument("", "Abc"),
                    new KeyValueArgument("verbosity", "Detailed"),
                    new KeyValueArgument("", "Xyz")
                },
                new Settings(Mock.Of<ICommandLineParser>(), Mock.Of<Func<ILog<Settings>>>()),
                true
            },
            new object[]
            {
                new[]
                {
                    new KeyValueArgument("", "Abc"),
                    new KeyValueArgument("verbosity2", "Detailed"),
                },
                new Settings(Mock.Of<ICommandLineParser>(), Mock.Of<Func<ILog<Settings>>>()),
                true
            },
            new object[]
            {
                new[]
                {
                    new KeyValueArgument("", "Abc"),
                    new KeyValueArgument("verbosity", "Detailed2"),
                },
                new Settings(Mock.Of<ICommandLineParser>(), Mock.Of<Func<ILog<Settings>>>()),
                true
            },
            new object[]
            {
                new[]
                {
                    new KeyValueArgument("", "Abc"),
                    new KeyValueArgument("timeout", "abc"),
                },
                new Settings(Mock.Of<ICommandLineParser>(), Mock.Of<Func<ILog<Settings>>>()),
                true
            },
            new object[]
            {
                new[]
                {
                    new KeyValueArgument("", "Abc"),
                    new KeyValueArgument("Var1", "Val1"),
                },
                new Settings(Mock.Of<ICommandLineParser>(), Mock.Of<Func<ILog<Settings>>>()),
                true
            },
            new object[]
            {
                new[]
                {
                    new KeyValueArgument("", "Abc"),
                    new KeyValueArgument("", "--"),
                    new KeyValueArgument("Var1", "Val1"),
                    new KeyValueArgument("", "--"),
                    new KeyValueArgument("Var2", "Val2"),
                },
                new Settings(Mock.Of<ICommandLineParser>(), Mock.Of<Func<ILog<Settings>>>()),
                true
            },
        };

        [Theory]
        [MemberData(nameof(TestData))]
        internal void ShouldSetup(KeyValueArgument[] args, Settings expectedSettings, bool expectedHasError)
        {
            // Given
            var actualHasError = false;
            var commandLineParser = new Mock<ICommandLineParser>();
            commandLineParser.Setup(i => i.Parse(new[] {"args"})).Returns(args);
            var log = new Mock<ILog<Settings>>();
            log.Setup(i => i.Error(It.IsAny<Text[]>())).Callback(() => actualHasError = true);
            var logFactory = new Mock<Func<ILog<Settings>>>();
            logFactory.Setup(i => i()).Returns(log.Object);
            var settings = new Settings(commandLineParser.Object, logFactory.Object);

            // When
            settings.Setup(new []{"args"});

            // Then
            actualHasError.ShouldBe(expectedHasError);
            if (!expectedHasError)
            {
                settings.ActivityName.ShouldBe(expectedSettings.ActivityName);
                settings.Timeout.ShouldBe(expectedSettings.Timeout);
                settings.Verbosity.ShouldBe(expectedSettings.Verbosity);
                settings.Inputs.ShouldBe(expectedSettings.Inputs);
            }
        }
    }
}
