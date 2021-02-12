namespace Flow.Core
{
    internal enum Tags
    {
        Default,
        TeamCity,

        // Directories
        WorkingDirectory,
        TempDirectory,
        TempFile,

        // ProcessListener
        StdOutErr,

        // ProcessFactory
        Base,
        Composite,

        // ProcessWrapper
        Docker,
        Script
    }
}
