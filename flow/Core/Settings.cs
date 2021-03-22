namespace Flow.Core
{
    using System;
    using System.Collections.Generic;
    using IoC;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class Settings: ISettings, ISettingsSetup
    {
        [NotNull] private readonly ICommandLineParser _commandLineParser;
        [NotNull] private readonly Func<ILog<Settings>> _log;
        private string _activityName;

        public Settings(
            [NotNull] ICommandLineParser commandLineParser,
            [NotNull] Func<ILog<Settings>> log)
        {
            _commandLineParser = commandLineParser ?? throw new ArgumentNullException(nameof(commandLineParser));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public string ActivityName
        {
            get => _activityName ?? "Default";
            internal set => _activityName = value;
        }

        public Verbosity Verbosity { get; internal set; }

        public TimeSpan Timeout { get; internal set; }

        public IDictionary<string, object> Inputs { get; } = new Dictionary<string, object>();

        public void Setup(Verbosity verbosity) => Verbosity = verbosity;

        public bool Setup(IEnumerable<string> args)
        {
            if (args == null) throw new ArgumentNullException(nameof(args));

            Reset();
            var log = _log();
            var hasError = false;
            var inputsStage = false;
            foreach (var arg in _commandLineParser.Parse(args))
            {
                switch (arg.Name.ToLowerInvariant())
                {
                    case "":
                        if (inputsStage)
                        {
                            log.Error($"Invalid argument \"{arg}\".");
                            hasError = true;
                            break;
                        }

                        if (arg.Value == "--")
                        {
                            inputsStage = true;
                            break;
                        }

                        if (_activityName == null)
                        {
                            _activityName = arg.Value;
                        }
                        else
                        {
                            log.Error($"An activity name should be specified once: {arg.Value}.");
                            hasError = true;
                        }

                        break;

                    case "timeout":
                        if (TimeSpan.TryParse(arg.Value, out var curTimeout))
                        {
                            Timeout = curTimeout;
                        }
                        else
                        {
                            log.Error($"Invalid timeout \"{arg.Value}\".");
                            hasError = true;
                        }

                        break;

                    case "verbosity":
                        if (Enum.TryParse<Verbosity>(arg.Value, true, out var curVerbosity))
                        {
                            Verbosity = curVerbosity;
                        }
                        else
                        {
                            log.Error($"Invalid verbosity \"{arg.Value}\".");
                            hasError = true;
                        }

                        break;

                    default:
                        if (inputsStage)
                        {
                            Inputs[arg.Name] = arg.Value;
                        }
                        else
                        {
                            log.Error($"Invalid argument \"{arg}\".");
                            hasError = true;
                        }

                        break;
                }

                if (hasError)
                {
                    Reset();
                    log.Info(() => new[]
                    {
                        "Arguments:",
                        Text.NewLine,
                        Text.Tab,
                        "<Activity Name>",
                        Text.NewLine,
                        Text.Tab,
                        "[--timeout] <Timeout>",
                        Text.NewLine,
                        Text.Tab,
                        "[--verbosity] <Verbosity Level>",
                        Text.NewLine,
                        Text.Tab,
                        "[--]",
                        Text.NewLine,
                        Text.Tab,
                        "[--argument name 1] <argument value 1>",
                        Text.NewLine,
                        Text.Tab,
                        "... [--argument name N] <argument value N>",
                    });

                    return false;
                }
            }

            return true;
        }

        private void Reset()
        {
            _activityName = null;
            Timeout = TimeSpan.MaxValue;
            Verbosity = Verbosity.Normal;
            Inputs.Clear();
        }
    }
}
