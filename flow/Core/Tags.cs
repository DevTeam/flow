namespace Flow.Core
{
    internal enum Tags
    {
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
