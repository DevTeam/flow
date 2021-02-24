namespace Flow.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using IoC;
    using static Tags;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class CurrentProcessFactory: IProcessFactory, IChain<IProcessWrapper>
    {
        [NotNull] private readonly IProcessFactory _processFactory;
        private readonly LinkedList<IProcessWrapper> _wrappers = new LinkedList<IProcessWrapper>();

        public CurrentProcessFactory(
            [NotNull, Tag(Base)] IProcessFactory processFactory) =>
            _processFactory = processFactory ?? throw new ArgumentNullException(nameof(processFactory));

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
                _wrappers.AddLast(wrapper);
            }
            
            return Disposable.Create(() =>
            {
                lock (_wrappers)
                {
                    _wrappers.Remove(wrapper);
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
            
            info = wrappers.Aggregate(info, (currentInfo, wrapper) => wrapper.Wrap(currentInfo));
            return _processFactory.Create(info);
        }
    }
}
