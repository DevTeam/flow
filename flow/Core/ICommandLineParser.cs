namespace Flow.Core
{
    using System.Collections.Generic;
    using IoC;

    internal interface ICommandLineParser
    {
        [NotNull] IEnumerable<KeyValueArgument> Parse([NotNull] IEnumerable<string> args);
    }
}