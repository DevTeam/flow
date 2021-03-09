namespace Flow.Tests
{
    using System;
    using System.Linq;
    using Castle.Core.Logging;
    using Core;
    using Moq;

    internal static class LogExtensions
    {
        public static void VerifyInfo<T>(this Mock<ILog<T>> log, params Text[] text) => 
            log.Verify(i => i.Info(It.Is<Func<Text[]>>(j => j().SequenceEqual(text))));

        public static void VerifyTrace<T>(this Mock<ILog<T>> log, params Text[] text) =>
            log.Verify(i => i.Trace(It.Is<Func<Text[]>>(j => j().SequenceEqual(text))));

        public static void VerifyError<T>(this Mock<ILog<T>> log, params Text[] text) =>
            log.Verify(i => i.Error(It.Is<Text[]>(j => j.SequenceEqual(text))));

        public static void VerifyWarning<T>(this Mock<ILog<T>> log, params Text[] text) =>
            log.Verify(i => i.Warning(It.Is<Text[]>(j => j.SequenceEqual(text))));
    }
}
