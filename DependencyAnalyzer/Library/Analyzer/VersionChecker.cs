using DependencyAnalyzer.Library.VersionStringParsing;

namespace DependencyAnalyzer.Library.Analyzer;

public static class VersionChecker
{
    public static string PerformCheck(string packageVersion, string parentExpectation)
    {
        var parsedVersion = Version.Parse(packageVersion);
        
        var expectation = VersionStringParser.Parse(parentExpectation);

        if (
            (
                expectation is { FromVersionInclusive: true, ToVersionInclusive: true }
                && expectation.FromVersion == parsedVersion
                && expectation.ToVersion == parsedVersion
            )
            ||
            (
                expectation is { FromVersionInclusive: true, ToVersionInclusive: false }
                && expectation.FromVersion == parsedVersion
                && expectation.ToVersion is null
            )
        )
        {
            return "[OK] Expectation matched reality exactly";
        }

        if (
            parsedVersion < expectation.FromVersion
            || (expectation.ToVersion is not null && parsedVersion > expectation.ToVersion)
            || (parsedVersion == expectation.FromVersion && !expectation.FromVersionInclusive)
            || (parsedVersion == expectation.ToVersion && !expectation.ToVersionInclusive)
        )
        {
            return $"[KO] Resolved version: {packageVersion} broke expected range: {parentExpectation}.";
        }

        if (
            parsedVersion.Major > expectation.FromVersion.Major &&
            (
                expectation.ToVersion is null
                ||
                parsedVersion <= expectation.ToVersion // out of range due to closed brackets already checked above
            )
        )
        {
            return $"[WARN] Resolved version: {packageVersion} is within expectation {expectation} but differs in major version.";
        }

        return $"[OK] Resolved version: {packageVersion} is within expectation: {parentExpectation}.";
    }
}