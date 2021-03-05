namespace Flow.Core
{
    using IoC;

    internal readonly struct Text
    {
        [CanBeNull] public readonly string Value;
        public readonly Color Color;

        public Text([CanBeNull] string value, Color color = Color.Default)
        {
            Value = value;
            Color = color;
        }
    }
}
