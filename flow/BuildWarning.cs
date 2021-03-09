namespace Flow
{
    using System;
    using IoC;

    public readonly struct BuildWarning
    {
        [NotNull] public readonly string Message;

        public BuildWarning([NotNull] string message) =>
            Message = message ?? throw new ArgumentNullException(nameof(message));

        public override string ToString() => Message;
    }
}
