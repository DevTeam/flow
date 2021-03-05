namespace Flow.Core
{
    using System;
    using System.Collections.Generic;
    using IoC;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class Log<T> : ILog<T>
    {
        private readonly Verbosity _verbosity;
        private readonly IColorfulStdOut _stdOut;
        private readonly Text _prefix;
        private readonly Text _emptyPrefix;

        public Log(
            Verbosity verbosity,
            [NotNull] IColorfulStdOut stdOut)
        {
            _verbosity = verbosity;
            _stdOut = stdOut ?? throw new ArgumentNullException(nameof(stdOut));
            var prefixStr = $"{typeof(T).Name} -> ";
            _prefix = new Text(prefixStr, Color.Trace);
            _emptyPrefix = new Text(new string(' ', prefixStr.Length));
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

            for (var i = 1; i < message.Length; i++)
            {
                var item = message[i];
                if (color != Color.Default && item.Color == Color.Default)
                {
                    message[i] = new Text(item.Value, color);
                }
            }

            if (showPrefix)
            {
                var line = new List<Text> { _prefix };
                foreach (var text in message)
                {
                    if (text.Value == Text.NewLine.Value)
                    {
                        _stdOut.WriteLine(line.ToArray());
                        line.Clear();
                        line.Add(_emptyPrefix);
                    }
                    else
                    {
                        line.Add(text);
                    }
                }

                if (line.Count > 0)
                {
                    _stdOut.WriteLine(line.ToArray());
                }
            }
            else
            {
                _stdOut.WriteLine(message);
            }
        }
    }
}