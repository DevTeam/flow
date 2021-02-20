namespace Flow.Core
{
    internal enum Tags
    {
        // Settings
        SeparatorChar,
        QuoteChar,
        DotnetExecutable,
        WindowsNewLineString,
        LinuxNewLineString,
        WindowsDirectorySeparatorString,
        LinuxDirectorySeparatorString,
        FlowVersionString,
        WslRootString,
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
        ShScriptWrapper,
        WslScriptWrapper,
        DotnetWrapper
    }
}
