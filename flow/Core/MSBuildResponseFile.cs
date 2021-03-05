namespace Flow.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using IoC;

    // ReSharper disable once InconsistentNaming
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class MSBuildResponseFile : IResponseFile
    {
        private readonly ILog<MSBuildResponseFile> _log;
        private readonly Verbosity _verbosity;
        [NotNull] private readonly ITeamCitySettings _teamCitySettings;
        [NotNull] private readonly IFileSystem _fileSystem;
        [NotNull] private readonly IConverter<MSBuildParameter, string> _paramConverter;
        [NotNull] private readonly Func<IPathNormalizer> _pathNormalizer;
        private readonly Path _rspPath;

        public MSBuildResponseFile(
            [NotNull] ILog<MSBuildResponseFile> log,
            Verbosity verbosity,
            [Tag(Tags.TempFile)] Path tempFilePath,
            [NotNull] ITeamCitySettings teamCitySettings,
            [NotNull] IFileSystem fileSystem,
            [NotNull] IConverter<MSBuildParameter, string> paramConverter,
            [NotNull] Func<IPathNormalizer> pathNormalizer)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _verbosity = verbosity;
            _teamCitySettings = teamCitySettings ?? throw new ArgumentNullException(nameof(teamCitySettings));
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            _paramConverter = paramConverter ?? throw new ArgumentNullException(nameof(paramConverter));
            _pathNormalizer = pathNormalizer ?? throw new ArgumentNullException(nameof(pathNormalizer));
            _rspPath = new Path(tempFilePath.Value + ".rsp");
        }

        public Path Create()
        {
            var pathNormalizer = _pathNormalizer();
            var args = new[]
            {
                "/noconsolelogger",
                $"/l:TeamCity.MSBuild.Logger.TeamCityMSBuildLogger,{pathNormalizer.Normalize(_teamCitySettings.MSBuildLogger).Value};TeamCity;verbosity={_verbosity}",
                "/p:VSTestLogger=logger://teamcity",
                _paramConverter.Convert(new MSBuildParameter("VSTestTestAdapterPath", $".;{pathNormalizer.Normalize(_teamCitySettings.VSTestLogger).Value}"))
            };

            _fileSystem.WriteLines(_rspPath, args);

            _log.Trace(() =>
            {
                var text = new List<Text> {_rspPath.Value, Text.NewLine };
                text.AddRange(args.Select(i => new Text(i)).Join(Text.NewLine));
                return text.ToArray();
            });

            return pathNormalizer.Normalize(_rspPath);
        }


    }
}