namespace Flow.Core
{
    using System;
    using System.Linq;
    using IoC;
    using static Tags;
    using OperatingSystem = OperatingSystem;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class WslWrapperService : IWslWrapperService, IProcessWrapper
    {
        [NotNull] private readonly IProcessChain _processChain;
        [NotNull] private readonly IProcessWrapper _wslProcessWrapper;

        public WslWrapperService(
            OperatingSystem operatingSystem,
            [NotNull] IProcessChain processChain,
            [NotNull, Tag(WslScriptWrapper)] IProcessWrapper wslProcessWrapper)
        {
            if (operatingSystem != OperatingSystem.Windows) throw new NotSupportedException();
            _processChain = processChain ?? throw new ArgumentNullException(nameof(processChain));
            _wslProcessWrapper = wslProcessWrapper;
        }

        public IDisposable Using()
        {
            return Disposable.Create(
                _processChain.Append(this),
                _processChain.Append(_wslProcessWrapper));
        }

        public ProcessInfo Wrap(ProcessInfo processInfo)
        {
            return new ProcessInfo(
                "wsl",
                processInfo.WorkingDirectory,
                new [] { new CommandLineArgument(processInfo.Executable.Value) }.Concat(processInfo.Arguments),
                Enumerable.Empty<EnvironmentVariable>());
        }
    }
}