namespace Flow.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class CommandLineParser: ICommandLineParser
    {
        public IEnumerable<KeyValueArgument> Parse(IEnumerable<string> args)
        {
            if (args == null) throw new ArgumentNullException(nameof(args));

            string name = null;
            foreach (var arg in args.Select(i => i.Trim()))
            {
                if (name == null)
                {
                    if (arg.Length > 2 && arg.StartsWith("--"))
                    {
                        name = arg.Substring(2, arg.Length - 2);
                    }
                    else
                    {
                        yield return new KeyValueArgument(string.Empty, arg);
                    }
                }
                else
                {
                    yield return new KeyValueArgument(name, arg);
                    name = null;
                }
            }
        }
    }
}