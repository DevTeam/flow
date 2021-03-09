namespace Flow.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using IoC;
    using static Tags;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class CompositeProcessFactory: IProcessFactory, IChain<IProcessWrapper>
    {
        private readonly ILog<CompositeProcessFactory> _log;
        [NotNull] private readonly IProcessFactory _processFactory;
        private readonly LinkedList<IProcessWrapper> _wrappers = new LinkedList<IProcessWrapper>();

        public CompositeProcessFactory(
            [NotNull] ILog<CompositeProcessFactory> log,
            [NotNull, Tag(Base)] IProcessFactory processFactory)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _processFactory = processFactory ?? throw new ArgumentNullException(nameof(processFactory));
        }

        public IProcessWrapper Current
        {
            get
            {
                lock (_wrappers)
                {
                    return _wrappers.Last.Value;
                }
            }
        }

        public IDisposable Append(IProcessWrapper wrapper)
        {
            if (wrapper == null) throw new ArgumentNullException(nameof(wrapper));

            lock (_wrappers)
            {
                _log.Trace(() => new Text($"Append wrapper {wrapper.GetType().Name}"));
                _wrappers.AddLast(wrapper);
            }
            
            return Disposable.Create(() =>
            {
                lock (_wrappers)
                {
                    _wrappers.Remove(wrapper);
                    _log.Trace(() => new Text($"Remove wrapper {wrapper.GetType().Name}"));
                }
            });
        }

        public IProcess Create(ProcessInfo info)
        {
            IProcessWrapper[] wrappers;
            lock (_wrappers)
            {
                wrappers = _wrappers.Reverse().ToArray();
            }
            
            info = wrappers.Aggregate(info, (currentInfo, wrapper) =>
            {
                LogInfo(currentInfo, "Before wrapping");
                var result = wrapper.Wrap(currentInfo);
                LogInfo(result, "After wrapping");
                return result;
            });

            return _processFactory.Create(info);
        }

        private void LogInfo(ProcessInfo currentInfo, string operation)
        {
            _log.Trace(() =>
            {
                return new[]
                {
                    new Text(operation),
                    Text.NewLine,
                    "FileName: ",
                    currentInfo.Executable.Value,
                    Text.NewLine,
                    "WorkingDirectory: ",
                    currentInfo.WorkingDirectory.Value,
                    Text.NewLine,
                    "Arguments: ",
                    string.Join(new string(Configuration.CommandLineArgumentsSeparatorChar, 1), currentInfo.Arguments),
                    Text.NewLine
                };
            });
        }
    }
}
