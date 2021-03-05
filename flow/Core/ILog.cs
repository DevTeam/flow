namespace Flow.Core
{
    using System;
    using IoC;

    internal interface ILog<T>
    {
        void Trace([NotNull] Func<Text[]> messageFactory);

        void Info([NotNull] Func<Text[]> messageFactory);

        void Warning([NotNull] params Text[] message);

        void Error([NotNull] params Text[] message);
    }
}
