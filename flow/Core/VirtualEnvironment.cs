namespace Flow.Core
{
    using System;
    using System.Collections.Generic;
    using IoC;
    using OperatingSystem = OperatingSystem;

    internal class VirtualEnvironment : IVirtualEnvironment
    {
        public VirtualEnvironment([NotNull] IEnvironment environment)
        {
            if (environment == null) throw new ArgumentNullException(nameof(environment));
            OperatingSystem = environment.OperatingSystem;
            PathNormalizer = environment.PathNormalizer;
            Variables = environment.Variables;
        }

        public OperatingSystem OperatingSystem { get; private set; }

        public IPathNormalizer PathNormalizer { get; private set; }

        public IEnumerable<EnvironmentVariable> Variables { get; private set; }

        public IVirtualEnvironment Set(OperatingSystem operatingSystem)
        {
            OperatingSystem = operatingSystem;
            return this;
        }

        public IVirtualEnvironment Set(IPathNormalizer pathNormalizer)
        {
            PathNormalizer = pathNormalizer ?? throw new ArgumentNullException(nameof(pathNormalizer));
            return this;
        }

        public IVirtualEnvironment Set(IEnumerable<EnvironmentVariable> variables)
        {
            Variables = variables ?? throw new ArgumentNullException(nameof(variables));
            return this;
        }
    }
}