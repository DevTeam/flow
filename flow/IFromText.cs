namespace Flow
{
    using System.Collections.Generic;
    using IoC;

    public interface IFromText<out T>
    {
        [NotNull] T Parse([NotNull] IEnumerator<char> text);
    }
}
