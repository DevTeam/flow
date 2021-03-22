namespace Flow.Core
{
    using System;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;

    // ReSharper disable once ClassNeverInstantiated.Global
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal class FlowIdGenerator : IFlowIdGenerator
    {
        public string NewFlowId() => Guid.NewGuid().ToString().Replace("-", string.Empty);
    }
}
