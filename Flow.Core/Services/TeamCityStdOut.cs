namespace Flow.Core.Services
{
    using System;

    internal class TeamCityStdOut: IStdOut
    {
        public void Write(string text) => Console.Write(text);
    }
}
