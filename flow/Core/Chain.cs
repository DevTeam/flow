namespace Flow.Core
{
    using System;
    using System.Collections.Generic;
    using IoC;
    using static Tags;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class Chain<T>: IChain<T>
    {
        [NotNull] private readonly LinkedList<T> _chain = new LinkedList<T>();

        public Chain([NotNull, Tag(Base)] T link) =>
            _chain.AddLast(link);

        public T Current
        {
            get
            {
                lock (_chain)
                {
                    return _chain.Last.Value;
                }
            }
        }

        public IDisposable Append(T link)
        {
            lock (_chain)
            {
                _chain.AddLast(link);
            }

            return Disposable.Create(() =>
            {
                lock (_chain)
                {
                    _chain.Remove(link);
                }
            });
        }
    }
}
