namespace Flow
{
    using System;
    using System.Collections.Generic;
    using IoC;

    public readonly struct BuildResult
    {
        public readonly bool Success;
        [NotNull] public readonly IReadOnlyCollection<BuildError> Errors;
        [NotNull] public readonly IReadOnlyCollection<BuildWarning> Warnings;

        public BuildResult(
            bool success,
            [NotNull] IReadOnlyCollection<BuildError> errors,
            [NotNull] IReadOnlyCollection<BuildWarning> warnings)
        {
            Success = success;
            Errors = errors ?? throw new ArgumentNullException(nameof(errors));
            Warnings = warnings ?? throw new ArgumentNullException(nameof(warnings));
        }
    }
}
