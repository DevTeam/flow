namespace Flow.Core
{
    using System;
    using IoC;
    using JetBrains.TeamCity.ServiceMessages;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class BuildLogFlow : IBuildLogFlow
    {
        [NotNull] private readonly IStdOut _stdOut;
        [NotNull] private readonly Func<IBuildLogFlow> _flowFactory;
        private int _tabs;
        private string _tabsString;

        public BuildLogFlow(
            [NotNull, Tag(Tags.Base)] IStdOut stdOut,
            [NotNull] Func<IBuildLogFlow> flowFactory)
        {
            _stdOut = stdOut ?? throw new ArgumentNullException(nameof(stdOut));
            _flowFactory = flowFactory ?? throw new ArgumentNullException(nameof(flowFactory));
        }

        public void Initialize(int tabs) => _tabs = tabs;

        public IBuildLogFlow CreateChild()
        {
            var childFlow = _flowFactory();
            childFlow.Initialize(_tabs);
            return childFlow;
        }

        public bool ProcessMessage(IServiceMessageProcessor processor, IBuildVisitor buildVisitor, IServiceMessage message)
        {
            var processed = false;

            var parseInternal = message.GetValue("tc:tags") == "tc:parseServiceMessagesInside";
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
                                buildVisitor.Visit(new BuildError(text));
                                break;

                            case "WARNING":
                                buildVisitor.Visit(new BuildWarning(text));
                                break;
                        }

                        if (!parseInternal || !processor.ProcessServiceMessages(text))
                        {
                            WriteLine(text);
                            processed = true;
                        }
                    }

                    break;

                case "blockopened":
                    WriteLine(message.GetValue("name"));
                    _tabs++;
                    _tabsString = new string(' ', _tabs * 2);
                    processed = true;
                    break;

                case "blockclosed":
                    _tabs--;
                    _tabsString = new string('\t', _tabs);
                    processed = true;
                    break;

                default:
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