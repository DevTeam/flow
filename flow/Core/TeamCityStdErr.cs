namespace Flow.Core
{
    using System;

    // ReSharper disable once ClassNeverInstantiated.Global
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal class TeamCityStdErr : IStdErr
    {
        public void WriteLine(string error)
        {
            if (error == null) throw new ArgumentNullException(nameof(error));
            Console.Error.WriteLine(error);
        }
    }
}