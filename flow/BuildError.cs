namespace Flow
{
    using System;
    using IoC;

    public readonly struct BuildError
    {
        [NotNull] public readonly string Message;

        public BuildError([NotNull] string message) => 
            Message = message ?? throw new ArgumentNullException(nameof(message));
    }
}
