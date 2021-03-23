namespace Flow.Core
{
    using System;
    using IoC;
    using JetBrains.TeamCity.ServiceMessages;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class BuildLogFlow : IBuildLogFlow
    {
        [NotNull] private readonly ILog<BuildLogFlow> _log;
        [NotNull] private readonly IStdOut _stdOut;
        [NotNull] private readonly Func<int, IBuildLogFlow> _flowFactory;
        private int _tabs;
        private string _tabsString;

        public BuildLogFlow(
            [NotNull] ILog<BuildLogFlow> log,
            [NotNull, Tag(Tags.Base)] IStdOut stdOut,
            [NotNull] Func<int, IBuildLogFlow> flowFactory,
            int tabs)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _stdOut = stdOut ?? throw new ArgumentNullException(nameof(stdOut));
            _flowFactory = flowFactory ?? throw new ArgumentNullException(nameof(flowFactory));
            _tabs = tabs;
        }

        public IBuildLogFlow CreateChild() => _flowFactory(_tabs);

        public bool ProcessMessage(IMessageProcessor processor, IBuildVisitor buildVisitor, IServiceMessage message)
        {
            var processed = false;

            switch (message.Name?.ToLowerInvariant())
            {
                case "message":
                    var text = message.GetValue("text");
                    if (text != null)
                    {
                        var status = message.GetValue("status")?.ToUpperInvariant();
                        switch (status)
                        {
                            case "ERROR":
                                _log.Trace(() => new[] { new Text($"Add error {text}") });
                                buildVisitor.Visit(new BuildError(text));
                                break;

                            case "WARNING":
                                _log.Trace(() => new[] { new Text($"Add warning {text}") });
                                buildVisitor.Visit(new BuildWarning(text));
                                break;
                        }
                    }

                    var parseInternal = message.GetValue("tc:tags")?.ToLowerInvariant() == "tc:parseservicemessagesinside";
                    if (!parseInternal || !processor.ProcessServiceMessages(text, buildVisitor))
                    {
                        WriteLine(text);
                    }

                    processed = true;
                    break;

                case "blockopened":
                    var blockName = message.GetValue("name");
                    _log.Trace(() => new []{ new Text($"Open block {blockName}")});
                    WriteLine(blockName);
                    _tabs++;
                    _tabsString = new string(' ', _tabs * Text.Tab.Value.Length);
                    processed = true;
                    break;

                case "blockclosed":
                    _log.Trace(() => new[] { new Text("Close block") });
                    _tabs--;
                    _tabsString = new string(' ', _tabs * Text.Tab.Value.Length);
                    processed = true;
                    break;
            }

            return processed;
        }

        private void WriteLine([CanBeNull] string line)
        {
            if (line != null)
            {
                _stdOut.WriteLine(_tabsString + line + "\x001B[0;37;40m");
            }
        }
    }
}