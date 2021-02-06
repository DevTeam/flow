﻿namespace Flow
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using IoC;
    using Services;

    public readonly struct Collection<T>: IEnumerable<T>, IFromText<Collection<T>>
        where T : IFromText<T>, new()
    {
        private static readonly T[] Empty = new T[0];
        private readonly IEnumerable<T> _items;

        public Collection([CanBeNull] [ItemNotNull] IEnumerable<T> items) =>
            _items = items?.ToArray() ?? Empty;

        public IEnumerator<T> GetEnumerator() => (_items ?? Empty).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public static implicit operator Collection<T>([NotNull] string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            using (var enumerator = text.GetEnumerator())
            {
                return new Collection<T>(enumerator.ParseEnumerable<T>());
            }
        }

        Collection<T> IFromText<Collection<T>>.Parse(IEnumerator<char> text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            var fromString = new T();
            var items = text.ParseEnumerable().Select(selector: str =>
            {
                using (var enumerator = str.GetEnumerator())
                {
                    return fromString.Parse(enumerator);
                }
            });

            return new Collection<T>(items);
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
