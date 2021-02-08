namespace Flow.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal static class FromTextExtensions
    {
        public static string Parse(this IEnumerator<char> text)
        {
            var sb = new StringBuilder();
            var quoted = false;
            var quote = '_';
            var finish = false;

            while (!finish && text.MoveNext())
            {
                switch (text.Current)
                {
                    case InternalEnvironment.CommandLineArgumentsQuoteChar:
                    case '\'':
                        if (!quoted && sb.Length == 0)
                        {
                            quote = text.Current;
                            quoted = true;
                            break;
                        }

                        if (quote == text.Current)
                        {
                            quote = '_';
                            quoted = false;
                            break;
                        }

                        sb.Append(text.Current);
                        break;

                    case InternalEnvironment.CommandLineArgumentsSeparatorChar:
                        if (!quoted)
                        {
                            if (sb.Length > 0)
                            {
                                finish = true;
                            }
                        }
                        else
                        {
                            sb.Append(text.Current);
                        }

                        break;

                    default:
                        sb.Append(text.Current);
                        break;
                }
            }

            if (quoted)
            {
                throw new ArgumentException(nameof(text));
            }

            return sb.ToString();
        }

        public static T Parse<T>(this IEnumerator<char> text)
            where T : IFromText<T>, new() =>
            new T().Parse(text);

        public static IEnumerable<string> ParseEnumerable(this IEnumerator<char> text)
        {
            do
            {
                var item = text.Parse();
                if (string.IsNullOrEmpty(item))
                {
                    break;
                }

                yield return item;
            } while (true);
        }

        public static IEnumerable<T> ParseEnumerable<T>(this IEnumerator<char> text)
            where T : IFromText<T>, new() =>
            text.ParseEnumerable()
                .Select(str =>
                {
                    using (var enumerator = str.GetEnumerator())
                    {
                        return enumerator.Parse<T>();
                    }
                });

        public static (string name, string value) ParseTuple(this IEnumerator<char> text)
        {
            var parts = text
                .ParseEnumerable()
                .Take(1)
                .SelectMany(i => i.Split('='))
                .ToArray();
            
            switch (parts.Length)
            {
                case 2:
                    return (parts[0], parts[1]);

                case 1:
                    return (parts[0], string.Empty);

                default:
                    return (string.Empty, string.Empty);
            }
        }
    }
}