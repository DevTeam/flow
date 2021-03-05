namespace Flow.Core
{
    using System;
    using IoC;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class Log<T> : ILog<T>
    {
        private readonly Verbosity _verbosity;
        private readonly IColorfulStdOut _stdOut;
        private readonly Text _prefix;

        public Log(
            Verbosity verbosity,
            [NotNull] IColorfulStdOut stdOut)
        {
            _verbosity = verbosity;
            _stdOut = stdOut ?? throw new ArgumentNullException(nameof(stdOut));
            _prefix = new Text($"{typeof(T).Name} -> ", Color.Trace);
        }

        public void Trace(Func<Text[]> message)
        {
            if (_verbosity > Verbosity.Detailed)
            {
                WriteLine(message(), true, Color.Trace);
            }
        }

        public void Info(Func<Text[]> message)
        {
            if (_verbosity > Verbosity.Minimal)
            {
                WriteLine(message(), false, Color.Trace);
            }
        }

        public void Warning(params Text[] message)
        {
            if (_verbosity > Verbosity.Quiet)
            {
                WriteLine(message, true, Color.Warning);
            }
        }

        public void Error(params Text[] message)
        {
            WriteLine(message, true, Color.Error);
        }

        private void WriteLine(Text[] message, bool showPrefix, Color color = Color.Default)
        {
            if (message.Length <= 0)
            {
                return;
            }

            if (showPrefix)
            {
                var text = new Text[message.Length + 1];
                text[0] = _prefix;
                Array.Copy(message, 0, text, 1, message.Length);
                message = text;
            }
            else
            {
                var text = new Text[message.Length];
                Array.Copy(message, text, message.Length);
                message = text;
            }

            for (var i = 1; i < message.Length; i++)
            {
                var item = message[i];
                if (color != Color.Default && item.Color == Color.Default)
                {
                    message[i] = new Text(item.Value, color);
                }
            }
            _stdOut.WriteLine(message);
        }
    }
}