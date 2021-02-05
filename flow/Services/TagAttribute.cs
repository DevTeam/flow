namespace Flow.Services
{
    using System;
    using IoC;

    [AttributeUsage(AttributeTargets.Parameter)]
    internal class TagAttribute: Attribute
    {
        [NotNull] public readonly object Tag;

        public TagAttribute([NotNull] object tag) =>
            Tag = tag ?? throw new ArgumentNullException(nameof(tag));
    }
}
