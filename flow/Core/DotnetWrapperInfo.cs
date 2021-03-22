namespace Flow.Core
{
    using System.Collections.Generic;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal readonly struct DotnetWrapperInfo
    {
        public readonly Path SolutionDirectory;
        public readonly Path DotnetExecutable;
        public readonly IEnumerable<EnvironmentVariable> EnvironmentVariables;


        public DotnetWrapperInfo(Path solutionDirectory, Path dotnetExecutable, IEnumerable<EnvironmentVariable> environmentVariables)
        {
            SolutionDirectory = solutionDirectory;
            DotnetExecutable = dotnetExecutable;
            EnvironmentVariables = environmentVariables;
        }
    }
}
