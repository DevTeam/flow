namespace Flow.Core
{
    using System;
    using IoC;

    // ReSharper disable once InconsistentNaming
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class MSBuildResponseFile : IResponseFile
    {
        [NotNull] private readonly ITeamCitySettings _teamCitySettings;
        [NotNull] private readonly IFileSystem _fileSystem;
        [NotNull] private readonly IConverter<MSBuildParameter, string> _paramConverter;
        private readonly Path _rspPath;

        public MSBuildResponseFile(
            [Tag(Tags.TempFile)] Path tempFilePath,
            [NotNull] ITeamCitySettings teamCitySettings,
            [NotNull] IFileSystem fileSystem,
            [NotNull] IConverter<MSBuildParameter, string> paramConverter)
        {
            _teamCitySettings = teamCitySettings ?? throw new ArgumentNullException(nameof(teamCitySettings));
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            _paramConverter = paramConverter ?? throw new ArgumentNullException(nameof(paramConverter));
            _rspPath = new Path(tempFilePath.Value + ".rsp");
        }

        public Path Create()
        {
            var hasFile = false;
            if (_teamCitySettings.IsUnderTeamCity)
            {
                _fileSystem.WriteLines(_rspPath, new[]
                {
                    "/noconsolelogger",
                    $"/l:TeamCity.MSBuild.Logger.TeamCityMSBuildLogger,{_teamCitySettings.MSBuildLogger.Value};TeamCity;verbosity=normal",
                    "/p:VSTestLogger=logger://teamcity",
                    _paramConverter.Convert(new MSBuildParameter("VSTestTestAdapterPath", $".;{_teamCitySettings.VSTestLogger.Value}"))
                });

                hasFile = true;
            }

            if (hasFile)
            {
                return _rspPath;
            }

            return new Path();
        }
    }
}