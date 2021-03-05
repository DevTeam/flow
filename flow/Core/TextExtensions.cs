namespace Flow.Core
{
    using System.Collections.Generic;

    internal static class TextExtensions
    {
        public static IEnumerable<Text> Join(this IEnumerable<Text> text, Text separator)
        {
            var first = true;
            foreach (var item in text)
            {
                if (!first)
                {
                    yield return separator;
                }
                else
                {
                    first = false;
                }

                yield return item;
            }
        }
    }
}
