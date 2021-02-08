namespace Flow.Core
{
    using System;

    [AttributeUsage(AttributeTargets.Parameter)]
    internal class TagAttribute: Attribute
    {
        public readonly Tags Tag;

        public TagAttribute(Tags tag) =>
            Tag = tag;
    }
}
