﻿namespace Flow.Core
{
    using System;

    // ReSharper disable once UnusedType.Global
    internal class StdOut: IStdOut
    {
        public void WriteLine(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            Console.WriteLine(text);
        }
    }
}
