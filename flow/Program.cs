#if NET48
namespace Flow
{
    using System;

    public static class Program
    {
        public static void Main(string[] args) =>
            Flows.Run(args.Length > 0 ? args[0] : "Default", TimeSpan.MaxValue);
    }
}
#endif