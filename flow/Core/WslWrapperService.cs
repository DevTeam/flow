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
        [NotNull] private readonly IChain<OperatingSystem> _osChain;
        [NotNull] private readonly IChain<IPathNormalizer> _pathNormalizerChain;
        [NotNull] private readonly IProcessWrapper _wslProcessWrapper;

        public WslWrapperService(
            [NotNull] IChain<IProcessWrapper> processChain,
            [NotNull] IChain<OperatingSystem> osChain,
            [NotNull] IChain<IPathNormalizer> pathNormalizerChain,
            [NotNull, Tag(WslScriptWrapper)] IProcessWrapper wslProcessWrapper)
        {
            _processChain = processChain ?? throw new ArgumentNullException(nameof(processChain));
            _osChain = osChain;
            _pathNormalizerChain = pathNormalizerChain ?? throw new ArgumentNullException(nameof(pathNormalizerChain));
            _wslProcessWrapper = wslProcessWrapper;
        }

        public IDisposable Using() =>
            Disposable.Create(
                _pathNormalizerChain.Append(this),
                _osChain.Append(OperatingSystem.Unix),
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