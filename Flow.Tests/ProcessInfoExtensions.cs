namespace Flow.Tests
{
    using Core;

    internal static class ProcessInfoExtensions
    {
        public static ProcessInfo Create(string executableName = "cmd.exe")
        {
            Path executable = executableName;
            Path workingDirectory = "wd";
            Enumerable<CommandLineArgument> arguments = "/? abc";
            Enumerable<EnvironmentVariable> variables = "var1=va1 var2=va2";

            return new ProcessInfo(executable, workingDirectory, arguments, variables);
        }
    }
}
