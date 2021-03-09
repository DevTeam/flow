namespace Flow.Core
{
    using System;
    using System.Collections.Generic;
    using IoC;
    using JetBrains.TeamCity.ServiceMessages.Read;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class MessageProcessor: IMessageProcessor
    {
        [NotNull] private readonly IServiceMessageParser _serviceMessageParser;
        [NotNull] private readonly Func<IBuildLogFlow> _flowFactory;
        [NotNull] private readonly Dictionary<string, IBuildLogFlow> _flows = new Dictionary<string, IBuildLogFlow>();

        public MessageProcessor(
            [NotNull] IServiceMessageParser serviceMessageParser,
            [NotNull] Func<IBuildLogFlow> flowFactory)
        {
            _serviceMessageParser = serviceMessageParser ?? throw new ArgumentNullException(nameof(serviceMessageParser));
            _flowFactory = flowFactory ?? throw new ArgumentNullException(nameof(flowFactory));
        }

        public bool ProcessServiceMessages(string text, IBuildVisitor buildVisitor)
        {
            if (buildVisitor == null) throw new ArgumentNullException(nameof(buildVisitor));

            if (text == null)
            {
                return false;
            }

            var processed = false;
            foreach (var message in _serviceMessageParser.ParseServiceMessages(text))
            {
                var flowId = message.GetValue("parent") ?? message.GetValue("flowId") ?? string.Empty;
                if (!_flows.TryGetValue(flowId, out var flow))
                {
                    flow = _flowFactory();
                    _flows.Add(flowId, flow);
                }

                switch (message.Name?.ToLowerInvariant())
                {
                    case "flowstarted":
                        _flows[message.GetValue("flowId")] = flow.CreateChild();
                        processed = true;
                        break;

                    case "flowfinished":
                        _flows.Remove(flowId);
                        processed = true;
                        break;

                    default:
                        processed |= flow.ProcessMessage(this, buildVisitor, message);
                        break;
                }
            }

            return processed;
        }
    }
}
