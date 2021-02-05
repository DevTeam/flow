namespace Flow.Core.Services
{
    using System;

    internal class TeamCityStdErr : IStdErr
    {
        public void Write(string error) => Console.Error.Write(error);
    }
}