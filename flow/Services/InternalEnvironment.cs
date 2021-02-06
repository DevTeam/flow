namespace Flow.Services
{
    using System;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class InternalEnvironment : IEnvironment
    {
        public Path WorkingDirectory => Environment.CurrentDirectory;
    }
}