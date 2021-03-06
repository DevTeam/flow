﻿namespace Flow.Core
{
    using System;

    // ReSharper disable once ClassNeverInstantiated.Global
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal class TeamCityStdOut: IStdOut
    {
        public void WriteLine(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            Console.WriteLine(text);
        }
    }
}
