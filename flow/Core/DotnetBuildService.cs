﻿namespace Flow.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using IoC;
    using static Tags;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class DotnetBuildService : IDotnetBuildService
    {
        private readonly Path _workingDirectory;
        [NotNull] private readonly IToolResolver _toolResolver;
        [NotNull] private readonly IProcessFactory _processFactory;
        [NotNull] private readonly IProcessListener<BuildResult> _processListener;
        [NotNull] private readonly IResponseFile _responseFile;

        public DotnetBuildService(
            [Tag(WorkingDirectory)] Path workingDirectory,
            [NotNull] IToolResolver toolResolver,
            [NotNull] IProcessFactory processFactory,
            [NotNull] IProcessListener<BuildResult> processListener,
            [NotNull] IResponseFile responseFile)
        {
            _workingDirectory = workingDirectory;
            _toolResolver = toolResolver ?? throw new ArgumentNullException(nameof(toolResolver));
            _processFactory = processFactory ?? throw new ArgumentNullException(nameof(processFactory));
            _processListener = processListener ?? throw new ArgumentNullException(nameof(processListener));
            _responseFile = responseFile ?? throw new ArgumentNullException(nameof(responseFile));
        }

        public BuildResult Execute(DotnetBuildInfo info)
        {
            if (!_toolResolver.TryResolve(Tool.Dotnet, out var dotnet))
            {
                throw new InvalidOperationException($"Cannot find {Tool.Dotnet}.");
            }

            var process = _processFactory.Create(
                new ProcessInfo(
                    dotnet,
                    _workingDirectory,
                    GetArgs().Concat(info.Arguments),
                    Enumerable.Empty<EnvironmentVariable>()));

            process.Run(_processListener);
            return _processListener.Result;
        }

        private IEnumerable<CommandLineArgument> GetArgs()
        {
            yield return "build";
            var rspFile = _responseFile.Create();
            if (!rspFile.IsEmpty)
            {
                yield return $"@{rspFile.Value}";
            }
        }
    }
}