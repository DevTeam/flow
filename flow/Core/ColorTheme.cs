namespace Flow.Core
{
    using System;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class ColorTheme : IColorTheme
    {
        public string GetAnsiColor(Color color)
        {
            switch (color)
            {
                case Color.Default:
                    return "0;37;40";

                case Color.Header:
                    return "1;37";

                case Color.Trace:
                    return "30;1";

                case Color.Success:
                    return "32;1";

                case Color.Warning:
                    return "31;1";

                case Color.Error:
                    return "33;1";

                default:
                    throw new ArgumentOutOfRangeException(nameof(color), color, null);
            }
        }
    }
}