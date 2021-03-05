﻿namespace Flow.Core
{
    using System;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;

    internal class FlowIdGenerator : IFlowIdGenerator
    {
        public string NewFlowId() =>Guid.NewGuid().ToString().Replace("-", string.Empty);
    }
}
