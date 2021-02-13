namespace Flow.Core
{
    internal enum Tags
    {
        // Settings
        SeparatorChar,
        QuoteChar,
        WindowsNewLineString,
        LinuxNewLineString,
        WindowsDirectorySeparatorString,
        LinuxDirectorySeparatorString,
        WorkingDirectory,
        TempDirectory,
        TempFile,
        TeamCity,

        // Process Listeners
        StdOutErr,

        // Process Factories
        Base,

        // Process Wrappers
        DockerWrapper,
        DockerEnvironment,
        DockerVolumes,
        CmdScriptWrapper,
        ShScriptWrapper
    }
}
