namespace Flow.Services
{
    using System;

    internal class InternalEnvironment : IEnvironment
    {
        public Path WorkingDirectory => Environment.CurrentDirectory;
    }
}