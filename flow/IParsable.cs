namespace Flow
{
    using IoC;

    public interface IParsable<out T>
    {
        [NotNull] T Parse([NotNull] string text);
    }
}
