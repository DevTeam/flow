namespace Flow.Core
{
    using System.Collections.Generic;
    using IoC;

    internal interface IDockerArgumentsProvider
    {
        [NotNull] IEnumerable<CommandLineArgument> GetArguments(DockerWrapperInfo wrapperInfo, ProcessInfo processInfo);
    }
}
