namespace Flow.Core
{
    using System;
    using System.Linq;
    using IoC;
    using static Tags;
    using OperatingSystem = OperatingSystem;
    using Path = Path;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class WslWrapperService : IWslWrapperService, IProcessWrapper, IPathNormalizer
    {
        [NotNull] private readonly IChain<IProcessWrapper> _processChain;
        [NotNull] private readonly IChain<IEnvironment> _envChain;
        [NotNull] private readonly IVirtualEnvironment _virtualEnvironment;
        [NotNull] private readonly IProcessWrapper _wslProcessWrapper;

        public WslWrapperService(
            [NotNull] IChain<IProcessWrapper> processChain,
            [NotNull] IChain<IEnvironment> envChain,
            [NotNull] IVirtualEnvironment virtualEnvironment,
            [NotNull, Tag(WslScriptWrapper)] IProcessWrapper wslProcessWrapper)
        {
            _processChain = processChain ?? throw new ArgumentNullException(nameof(processChain));
            _envChain = envChain ?? throw new ArgumentNullException(nameof(envChain));
            _virtualEnvironment = virtualEnvironment ?? throw new ArgumentNullException(nameof(virtualEnvironment));
            _wslProcessWrapper = wslProcessWrapper;
        }

        public IDisposable Using() =>
            Disposable.Create(
                _envChain.Append(_virtualEnvironment.Set(OperatingSystem.Unix).Set(this)),
                _processChain.Append(this),
                _processChain.Append(_wslProcessWrapper));

        public ProcessInfo Wrap(ProcessInfo processInfo) =>
            new ProcessInfo(
                "wsl",
                processInfo.WorkingDirectory,
                new [] { new CommandLineArgument(processInfo.Executable.Value) }.Concat(processInfo.Arguments),
                Enumerable.Empty<EnvironmentVariable>());

        public Path Normalize(Path path)
        {
            var fullPath = System.IO.Path.GetFullPath(path.Value);
            var root = System.IO.Path.GetPathRoot(fullPath);
            var newRoot = "/mnt/" + root.Replace(":\\", "/").ToLowerInvariant();
            return path.Value
                .Replace(root, newRoot)
                .Replace("\\", "/");
        }
    }
}