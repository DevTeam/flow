namespace Flow.Core
{
    using System;
    using IoC;
    using OperatingSystem = OperatingSystem;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class ToolResolver : IToolResolver
    {
        [NotNull] private readonly Func<OperatingSystem> _operatingSystem;
        [NotNull] private readonly Func<ILocator> _locator;

        public ToolResolver(
            [NotNull] Func<OperatingSystem> operatingSystem,
            [NotNull] Func<ILocator> locator)
        {
            _operatingSystem = operatingSystem;
            _locator = locator ?? throw new ArgumentNullException(nameof(locator));
        }

        public bool TryResolve(Tool tool, out Path path)
        {
            Path executable;

            var os = _operatingSystem();
            switch (tool)
            {
                case Tool.Dotnet:
                    executable = os == OperatingSystem.Windows ? "dotnet.exe" : "dotnet";
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(tool), tool, null);
            }

            return _locator().TryFind(executable, out path);
        }
    }
}