namespace Flow.Core
{
    using System;
    using System.Collections.Generic;
    using IoC;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class Log<T> : ILog<T>
    {
        private readonly Func<Verbosity> _verbosity;
        private readonly IColorfulStdOut _stdOut;
        private readonly Text _prefix;
        private readonly Text _emptyPrefix;

        public Log(
            Func<Verbosity> verbosity,
            [NotNull] IColorfulStdOut stdOut)
        {
            _verbosity = verbosity;
            _stdOut = stdOut ?? throw new ArgumentNullException(nameof(stdOut));
            var prefixStr = $"{typeof(T).Name} -> ";
            _prefix = new Text(prefixStr, Color.Trace);
            _emptyPrefix = new Text(new string(' ', prefixStr.Length));
        }

        public void Trace(Func<Text[]> messageFactory)
        {
            if (messageFactory == null) throw new ArgumentNullException(nameof(messageFactory));
            if (_verbosity() > Verbosity.Detailed)
            {
                WriteLine(messageFactory(), true, Color.Trace);
            }
        }

        public void Info(Func<Text[]> messageFactory)
        {
            if (messageFactory == null) throw new ArgumentNullException(nameof(messageFactory));
            if (_verbosity() > Verbosity.Minimal)
            {
                WriteLine(messageFactory(), false, Color.Trace);
            }
        }

        public void Warning(params Text[] message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (_verbosity() > Verbosity.Quiet)
            {
                WriteLine(message, _verbosity() > Verbosity.Normal, Color.Warning);
            }
        }

        public void Error(params Text[] message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            WriteLine(message, _verbosity() > Verbosity.Normal, Color.Error);
        }

        private void WriteLine(Text[] message, bool showPrefix, Color color = Color.Default)
        {
            if (message.Length <= 0)
            {
                return;
            }

            for (var i = 0; i < message.Length; i++)
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