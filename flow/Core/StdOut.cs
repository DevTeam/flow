namespace Flow.Core
{
    using System;

    // ReSharper disable once UnusedType.Global
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal class StdOut: IStdOut
    {
        public void WriteLine(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            Console.WriteLine(text);
        }
    }
}
