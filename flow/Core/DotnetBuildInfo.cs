namespace Flow.Core
{
    using System.Collections.Generic;
    using IoC;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal readonly struct DotnetBuildInfo
    {
        [NotNull] public readonly IEnumerable<CommandLineArgument> Arguments;

        public DotnetBuildInfo([NotNull] IEnumerable<CommandLineArgument> arguments)
        {
            Arguments = arguments;
        }
    }
}
