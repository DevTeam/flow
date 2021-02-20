namespace Flow.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using IoC;
    using static Tags;
    using OperatingSystem = OperatingSystem;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class CmdProcessWrapper: IProcessWrapper
    {
        [NotNull] private readonly Func<Path> _tempFilePathFactory;
        [NotNull] private readonly IFileSystem _fileSystem;
        [NotNull] private readonly IConverter<IEnumerable<CommandLineArgument>, string> _argumentsToStringConverter;
        [NotNull] private readonly string _quote;
        [NotNull] private readonly string _separator;

        public CmdProcessWrapper(
            [NotNull] [Tag(TempFile)] Func<Path> tempFilePathFactory,
            [NotNull] IEnvironment environment,
            [NotNull] IFileSystem fileSystem,
            [NotNull] IConverter<IEnumerable<CommandLineArgument>, string> argumentsToStringConverter)
        {
            _tempFilePathFactory = tempFilePathFactory ?? throw new ArgumentNullException(nameof(tempFilePathFactory));
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            _argumentsToStringConverter = argumentsToStringConverter ?? throw new ArgumentNullException(nameof(argumentsToStringConverter));
            _separator = new string(environment.CommandLineArgumentsSeparator, 1);
            _quote = new string(environment.CommandLineArgumentsQuote, 1);
        }

        public ProcessInfo Wrap(ProcessInfo processInfo)
        {
            var cmdPath = new Path(_tempFilePathFactory().Value + ".cmd");
            _fileSystem.WriteLines(cmdPath, GetCmdContent(processInfo), OperatingSystem.Windows);
            return new ProcessInfo(
                "cmd.exe",
                processInfo.WorkingDirectory,
                new CommandLineArgument[]
                {
                    "/C",
                    cmdPath.Value
                },
                Enumerable.Empty<EnvironmentVariable>());
        }

        private IEnumerable<string> GetCmdContent(ProcessInfo processInfo)
        {
            yield return "@echo off";
            var hasWorkingDirectory = !processInfo.WorkingDirectory.IsEmpty;
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
