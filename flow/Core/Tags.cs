namespace Flow.Core
{
    internal enum Tags
    {
        // Settings
        ArgumentsSeparatorChar,
        ArgumentsQuoteChar,
        NewLineString,
        DirectorySeparatorString,
        FlowVersionString,
        WorkingDirectory,
        TempDirectory,
        TempFile,
        TeamCity,
        Base,

        // Process Listeners
        StdOutErr,

        // Process Wrappers
        DockerWrapper,
        DockerEnvironment,
        DockerVolumes,
        CmdScriptWrapper,
        ShScriptWrapper,
        WslScriptWrapper
    }
}
