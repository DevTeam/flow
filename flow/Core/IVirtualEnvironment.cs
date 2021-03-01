namespace Flow.Core
{
    using System.Collections.Generic;
    using IoC;

    internal interface IVirtualEnvironment: IEnvironment
    {
        [NotNull] IVirtualEnvironment Set(OperatingSystem operatingSystem);

        [NotNull] IVirtualEnvironment Set([NotNull] IPathNormalizer pathNormalizer);

        [NotNull] IVirtualEnvironment Set([NotNull] IEnumerable<EnvironmentVariable> variables);
    }
}