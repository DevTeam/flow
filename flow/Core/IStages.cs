namespace Flow.Core
{
    using System.Collections;
    using System.Collections.Generic;

    [Public]
    internal interface IStages
    {
        void Before();

        void After(IDictionary<string, object> results);
    }
}
