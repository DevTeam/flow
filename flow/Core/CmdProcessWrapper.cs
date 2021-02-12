namespace Flow.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using IoC;
    using static Tags;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class CmdProcessWrapper: IProcessWrapper
    {
        private readonly Path _cmdPath;
        [NotNull] private readonly IFileSystem _fileSystem;
        [NotNull] private readonly IConverter<IEnumerable<CommandLineArgument>, string> _argumentsToStringConverter;
        [NotNull] private readonly string _quote;
        [NotNull] private readonly string _separator;

        public CmdProcessWrapper(
            [Tag(TempFile)] Path tempFilePath,
            [NotNull] IEnvironment environment,
            [NotNull] IFileSystem fileSystem,
            [NotNull] IConverter<IEnumerable<CommandLineArgument>, string> argumentsToStringConverter)
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            _argumentsToStringConverter = argumentsToStringConverter ?? throw new ArgumentNullException(nameof(argumentsToStringConverter));
            _cmdPath = tempFilePath + ".cmd";
            _separator = new string(environment.CommandLineArgumentsSeparator, 1);
            _quote = new string(environment.CommandLineArgumentsQuote, 1);
        }

        public ProcessInfo Wrap(ProcessInfo processInfo)
        {
            _fileSystem.WriteLines(_cmdPath, GetCmdContent(processInfo));
            return new ProcessInfo(
                "cmd.exe",
                processInfo.WorkingDirectory,
                GetArgs(),
                Enumerable.Empty<EnvironmentVariable>());
        }

        private IEnumerable<CommandLineArgument> GetArgs()
        {
            yield return "/C";
            yield return new CommandLineArgument(_cmdPath.Value, CommandLineArgumentType.Path);
        }

        private IEnumerable<string> GetCmdContent(ProcessInfo processInfo)
        {
            yield return @"echo off";
            var hasWorkingDirectory = !string.IsNullOrWhiteSpace(processInfo.WorkingDirectory.Value);
            if (hasWorkingDirectory)
            {
                yield return $"pushd {_quote}{processInfo.WorkingDirectory.Value}{_quote}";
            }

            foreach (var variable in processInfo.Variables)
            {
                yield return $"set {_quote}{variable.Name}={variable.Value}{_quote}";
            }

            yield return $"{_quote}{processInfo.Executable.Value}{_quote}{_separator}{_argumentsToStringConverter.Convert(processInfo.Arguments)}";
            yield return "set exitCode = %errorlevel%";
            if (hasWorkingDirectory)
            {
                yield return "popd";
            }

            yield return "exit /b %exitCode%";
        }
    }
}
