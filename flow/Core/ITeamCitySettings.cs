namespace Flow.Core
{
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal interface ITeamCitySettings
    {
        bool IsUnderTeamCity { get; }

        Path MSBuildLogger { get; }

        Path VSTestLogger { get; }
    }
}
