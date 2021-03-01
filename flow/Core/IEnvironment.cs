namespace Flow.Core
{
    using System.Collections.Generic;
    using IoC;
    using OperatingSystem = OperatingSystem;

    internal interface IEnvironment
    {
        OperatingSystem OperatingSystem { get; }

        [NotNull] IPathNormalizer PathNormalizer { get; }

        [NotNull] IEnumerable<EnvironmentVariable> Variables { get; }
    }
}