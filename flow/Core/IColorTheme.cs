namespace Flow.Core
{
    using IoC;

    internal interface IColorTheme
    {
        [NotNull] string GetAnsiColor(Color color);
    }
}
