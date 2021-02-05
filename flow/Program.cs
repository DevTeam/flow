#if NET48
namespace Flow
{
    using System;

    public static class Program
    {
        public static void Main() =>
            new Default().Run(TimeSpan.MaxValue);
    }
}
#endif