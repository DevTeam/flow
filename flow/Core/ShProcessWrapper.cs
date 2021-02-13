namespace Flow.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using IoC;
    using static Tags;
    using OperatingSystem = OperatingSystem;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class ShProcessWrapper: IProcessWrapper
    {
        [NotNull] private readonly Func<Path> _tempFilePathFactory;
        [NotNull] private readonly IFileSystem _fileSystem;
        [NotNull] private readonly IPathNormalizer _pathNormalizer;
        [NotNull] private readonly IConverter<IEnumerable<CommandLineArgument>, string> _argumentsToStringConverter;
        [NotNull] private readonly string _quote;
        [NotNull] private readonly string _separator;

        public ShProcessWrapper(
            [NotNull][Tag(TempFile)] Func<Path> tempFilePathFactory,
            [NotNull] IEnvironment environment,
            [NotNull] IFileSystem fileSystem,
            [NotNull] IPathNormalizer pathNormalizer,
            [NotNull] IConverter<IEnumerable<CommandLineArgument>, string> argumentsToStringConverter)
        {
            _tempFilePathFactory = tempFilePathFactory ?? throw new ArgumentNullException(nameof(tempFilePathFactory));
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            _pathNormalizer = pathNormalizer;
            _argumentsToStringConverter = argumentsToStringConverter ?? throw new ArgumentNullException(nameof(argumentsToStringConverter));
            _separator = new string(environment.CommandLineArgumentsSeparator, 1);
            _quote = new string(environment.CommandLineArgumentsQuote, 1);
        }

        public ProcessInfo Wrap(ProcessInfo processInfo)
        {
            var shPath = new Path(_tempFilePathFactory().Value + ".sh");
            _fileSystem.WriteLines(shPath, GetCmdContent(processInfo), OperatingSystem.Unix);
            shPath = _pathNormalizer.Normalize(shPath.Value, OperatingSystem.Unix).Value;
            return new ProcessInfo(
                "sh",
                processInfo.WorkingDirectory,
                new[]
                {
                    "-c",
                    new CommandLineArgument($"chmod +x {shPath.Value} && {shPath.Value}")
                }, 
                Enumerable.Empty<EnvironmentVariable>());
        }

        private IEnumerable<string> GetCmdContent(ProcessInfo processInfo)
        {
            yield return "#!/bin/bash";
            var hasWorkingDirectory = !string.IsNullOrWhiteSpace(processInfo.WorkingDirectory.Value);
            if (hasWorkingDirectory)
            {
                yield return $"cd {_quote}{processInfo.WorkingDirectory.Value}{_quote}";
            }

            foreach (var variable in processInfo.Variables)
            {
                yield return $"export {_quote}{variable.Name}={variable.Value}{_quote}";
            }

            yield return $"{_quote}{processInfo.Executable.Value}{_quote}{_separator}{_argumentsToStringConverter.Convert(processInfo.Arguments)}";
        }
    }
}