namespace Flow.Core.Services
{
    using System;

    internal class StdOut: IStdOut
    {
        public void Write(string text) => Console.Write(text);
    }
}
