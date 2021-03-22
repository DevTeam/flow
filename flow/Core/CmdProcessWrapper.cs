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
        [NotNull] private readonly IFileSystem _fileSystem;
        [NotNull] private readonly IConverter<IEnumerable<CommandLineArgument>, string> _argumentsToStringConverter;
        [NotNull] private readonly string _quote;
        [NotNull] private readonly string _separator;
        private readonly Path _cmdPath;

        public CmdProcessWrapper(
            [Tag(TempFile)] Path tempFilePath,
            [Tag(ArgumentsSeparatorChar)] char argumentsSeparator,
            [Tag(ArgumentsQuoteChar)] char argumentsQuote,
            [NotNull] IFileSystem fileSystem,
            [NotNull] IConverter<IEnumerable<CommandLineArgument>, string> argumentsToStringConverter)
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            _argumentsToStringConverter = argumentsToStringConverter ?? throw new ArgumentNullException(nameof(argumentsToStringConverter));
            _separator = new string(argumentsSeparator, 1);
            _quote = new string(argumentsQuote, 1);
            _cmdPath = new Path(tempFilePath.Value + ".cmd");
        }

        public ProcessInfo Wrap(ProcessInfo processInfo)
        {
            _fileSystem.WriteLines(_cmdPath, GetCmdContent(processInfo));
            return new ProcessInfo(
                "cmd.exe",
                processInfo.WorkingDirectory,
                new CommandLineArgument[]
                {
                    "/C",
                    _cmdPath.Value
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
            yield return "set exitCode=%errorlevel%";
            if (hasWorkingDirectory)
            {
                yield return "popd";
            }

            yield return "exit /b %exitCode%";
        }
    }
}
