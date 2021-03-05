#if DEBUG
namespace Flow
{
    using System;
    using System.Collections.Generic;

    public static class Program
    {
        public static void Main(string[] args) =>
            Flows.Run(args.Length > 0 ? args[0] : "Default", new Dictionary<string, object>(), TimeSpan.MaxValue);
    }
}
#endif