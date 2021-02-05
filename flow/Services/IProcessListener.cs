namespace Flow.Services
{
    using System.Diagnostics;
    using IoC;

    internal interface IProcessListener
    {
        void OnStart([NotNull] ProcessStartInfo startInfo);

        void OnStdOut([NotNull] string line);

        void OnStdErr([NotNull] string line);

        void OnExitCode(ExitCode exitCode);
    }
}
