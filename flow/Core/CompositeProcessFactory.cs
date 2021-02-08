namespace Flow.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using IoC;
    using static Tags;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class CompositeProcessFactory: IProcessFactory, IProcessChain
    {
        [NotNull] private readonly IProcessFactory _processFactory;
        private readonly IList<IProcessWrapper> _wrappers = new List<IProcessWrapper>();

        public CompositeProcessFactory(
            [NotNull, Tag(Base)] IProcessFactory processFactory)
        {
            _processFactory = processFactory ?? throw new ArgumentNullException(nameof(processFactory));
        }

        public IDisposable Append(IProcessWrapper wrapper)
        {
            if (wrapper == null) throw new ArgumentNullException(nameof(wrapper));

            lock (_wrappers)
            {
                _wrappers.Add(wrapper);
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
