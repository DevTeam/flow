namespace Flow
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using IoC;

    public readonly struct Collection<T>: IEnumerable<T>
        where T : IParsable<T>, new()
    {
        private static readonly T[] Empty = new T[0];
        private readonly IEnumerable<T> _items;

        public Collection([CanBeNull] [ItemNotNull] IEnumerable<T> items) =>
            _items = items?.ToArray() ?? Empty;

        public IEnumerator<T> GetEnumerator() => (_items ?? Empty).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public static implicit operator Collection<T>([NotNull] string items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            var parsable = new T();
            return new Collection<T>(items.Split(' ').Select(item => parsable.Parse(item)));
        }

        public override string ToString()
        {
            if (_items != null && _items.Any())
            {
                return string.Join(" ", _items);
            }

            return "empty";
        }
    }
}
