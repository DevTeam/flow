namespace Flow.Core
{
    using System.Collections.Generic;
    using IoC;

    interface ISettingsSetup
    {
        void Setup(Verbosity verbosity);

        bool Setup([NotNull] IEnumerable<string> args);
    }
}
