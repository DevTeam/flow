namespace Flow.Core.Services
{
    using System;

    internal class StdErr : IStdErr
    {
        public void Write(string error) => Console.Error.Write(error);
    }
}