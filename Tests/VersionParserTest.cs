using DependencyAnalyzer.Library.VersionStringParsing;
using NUnit.Framework;

namespace Tests;

public class VersionParserTest
{
    [Test]
    [TestCaseSource(nameof(ParseVersionsTcd))]
    public void ParseVersion(string version, VersionRange expectedVersionRange)
    {
        // When
        var r = VersionStringParser.Parse(version);
        
        // Then
        
        Assert.That(r.FromVersionInclusive, Is.EqualTo(expectedVersionRange.FromVersionInclusive));
        Assert.That(r.FromVersion, Is.EqualTo(expectedVersionRange.FromVersion));
        Assert.That(r.ToVersionInclusive, Is.EqualTo(expectedVersionRange.ToVersionInclusive));
        Assert.That(r.ToVersion, Is.EqualTo(expectedVersionRange.ToVersion));
    }

    public static IEnumerable<TestCaseData> ParseVersionsTcd()
    {
        yield return new TestCaseData(
            "2.9.6",
            new VersionRange
            {
                FromVersionInclusive = true,
                FromVersion = new Version(2,9,6),
                ToVersionInclusive = false,
                ToVersion = null
            }
        );
        
        yield return new TestCaseData(
            "[2.9.6]",
            new VersionRange
            {
                FromVersionInclusive = true,
                FromVersion = new Version(2,9,6),
                ToVersionInclusive = true,
                ToVersion = new Version(2,9,6)
            }
        );
        
        yield return new TestCaseData(
            "[2.9.6,3.7.4]",
            new VersionRange
            {
                FromVersionInclusive = true,
                FromVersion = new Version(2,9,6),
                ToVersionInclusive = true,
                ToVersion = new Version(3,7,4)
            }
        );
        
        yield return new TestCaseData(
            "[2.9.6, 3.7.4]",
            new VersionRange
            {
                FromVersionInclusive = true,
                FromVersion = new Version(2,9,6),
                ToVersionInclusive = true,
                ToVersion = new Version(3,7,4)
            }
        );
        
        yield return new TestCaseData(
            "[2.9.6, 3.7.4)",
            new VersionRange
            {
                FromVersionInclusive = true,
                FromVersion = new Version(2,9,6),
                ToVersionInclusive = false,
                ToVersion = new Version(3,7,4)
            }
        );
        
        yield return new TestCaseData(
            "(2.9.6, 3.7.4)",
            new VersionRange
            {
                FromVersionInclusive = false,
                FromVersion = new Version(2,9,6),
                ToVersionInclusive = false,
                ToVersion = new Version(3,7,4)
            }
        );
        
        yield return new TestCaseData(
            "[2.9.6,)",
            new VersionRange
            {
                FromVersionInclusive = true,
                FromVersion = new Version(2,9,6),
                ToVersionInclusive = false,
                ToVersion = null
            }
        );
        
        yield return new TestCaseData(
            "(2.9.6,)",
            new VersionRange
            {
                FromVersionInclusive = false,
                FromVersion = new Version(2,9,6),
                ToVersionInclusive = false,
                ToVersion = null
            }
        );
        
        yield return new TestCaseData(
            "(,3.7.4]",
            new VersionRange
            {
                FromVersionInclusive = false,
                FromVersion = new Version(0,0,0),
                ToVersionInclusive = true,
                ToVersion = new Version(3,7,4)
            }
        );
    }
}