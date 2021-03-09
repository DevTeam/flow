namespace Flow.Core
{
    using System;
    using System.Collections.Generic;
    using IoC;
    using JetBrains.TeamCity.ServiceMessages.Read;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class MessageProcessor: IMessageProcessor
    {
        [NotNull] private readonly ILog<MessageProcessor> _log;
        [NotNull] private readonly IServiceMessageParser _serviceMessageParser;
        [NotNull] private readonly Func<IBuildLogFlow> _flowFactory;
        [NotNull] private readonly Dictionary<string, IBuildLogFlow> _flows = new Dictionary<string, IBuildLogFlow>();

        public MessageProcessor(
            [NotNull] ILog<MessageProcessor> log,
            [NotNull] IServiceMessageParser serviceMessageParser,
            [NotNull] Func<IBuildLogFlow> flowFactory)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _serviceMessageParser = serviceMessageParser ?? throw new ArgumentNullException(nameof(serviceMessageParser));
            _flowFactory = flowFactory ?? throw new ArgumentNullException(nameof(flowFactory));
        }

        internal IDictionary<string, IBuildLogFlow> Flows => _flows;

        public bool ProcessServiceMessages(string text, IBuildVisitor buildVisitor)
        {
            if (buildVisitor == null) throw new ArgumentNullException(nameof(buildVisitor));

            if (string.IsNullOrWhiteSpace(text))
            {
                _log.Trace(() => new []{new Text("Message is empty.")});
                return false;
            }

            var processed = false;
            foreach (var message in _serviceMessageParser.ParseServiceMessages(text))
            {
                _log.Trace(() => new[] { new Text($"Start message processing {message}") });

                var flowId = message.GetValue("parent") ?? message.GetValue("flowId") ?? string.Empty;
                if (!_flows.TryGetValue(flowId, out var flow))
                {
                    _log.Trace(() => new[] { new Text($"Create flow {flowId}") });
                    flow = _flowFactory();
                    _flows.Add(flowId, flow);
                }
                else
                {
                    _log.Trace(() => new[] { new Text($"Use existing flow {flowId}") });
                }

                switch (message.Name?.ToLowerInvariant())
                {
                    case "flowstarted":
                        _log.Trace(() => new[] { new Text($"Create child flow {flowId}") });
                        _flows[message.GetValue("flowId")] = flow.CreateChild();
                        processed = true;
                        break;

                    case "flowfinished":
                        _log.Trace(() => new[] { new Text($"Remove flow {flowId}") });
                        _flows.Remove(flowId);
                        processed = true;
                        break;

                    default:
                        var result = flow.ProcessMessage(this, buildVisitor, message);
                        _log.Trace(() => new[] { new Text($"Processed: {result}") });
                        processed |= result;
                        break;
                }

                _log.Trace(() => new[] { new Text($"Finish message processing  {message}") });
            }

            return processed;
        }
    }
}
