namespace Flow.Services
{
    using System;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class StdErr : IStdErr
    {
        public void WriteLine(string error)
        {
            if (error == null) throw new ArgumentNullException(nameof(error));
            Console.Error.WriteLine(error);
        }
    }
}