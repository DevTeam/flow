namespace flow
{
    using System;
    using Flow.Core;

    public static class Program
    {
        public static void Main(string[] args) =>
            Activities.Run(args.Length > 0 ? args[0] : "Default", TimeSpan.MaxValue);
    }
}
