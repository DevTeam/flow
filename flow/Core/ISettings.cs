namespace Flow.Core
{
    using System;
    using System.Collections.Generic;

    internal interface ISettings
    {
        string ActivityName { get; }

        Verbosity Verbosity { get; }

        TimeSpan Timeout { get; }

        IDictionary<string, object> Inputs { get; }
    }
}
