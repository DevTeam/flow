namespace Flow.Core
{
    using System;
    using System.Text;
    using IoC;

    internal class ColorfulStdOut : IColorfulStdOut
    {
        [NotNull] private readonly IStdOut _stdOut;
        [NotNull] private readonly IColorTheme _colorTheme;

        public ColorfulStdOut(
            [NotNull] IStdOut stdOut,
            [NotNull] IColorTheme colorTheme)
        {
            _stdOut = stdOut ?? throw new ArgumentNullException(nameof(stdOut));
            _colorTheme = colorTheme ?? throw new ArgumentNullException(nameof(colorTheme));
        }

        public void WriteLine(params Text[] text)
        {
            var sb = new StringBuilder();
            var lastColor = Color.Default;
            foreach (var item in text)
            {
                if (item.Color != lastColor)
                {
                    lastColor = item.Color;
                    sb.Append($"\x001B[{_colorTheme.GetAnsiColor(lastColor)}m");
                }

                sb.Append(item.Value);
            }

            sb.Append($"\x001B[{_colorTheme.GetAnsiColor(Color.Default)}m");
            _stdOut.WriteLine(sb.ToString());
        }
    }
}