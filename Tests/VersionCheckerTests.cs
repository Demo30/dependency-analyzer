using DependencyAnalyzer.Library.Analyzer;
using NUnit.Framework;

namespace Tests;

public class VersionCheckerTests
{
    [Test]
    [TestCaseSource(nameof(GetVersionsTestTcd))]
    public void CheckVersionTest(string actualVersion, string versionRange, string expectedTag)
    {
        // When
        var r = VersionChecker.PerformCheck(actualVersion, versionRange);
        
        // Then
        Assert.That(r.Contains(expectedTag));
    }

    public static IEnumerable<TestCaseData> GetVersionsTestTcd()
    {
        yield return new TestCaseData(
            "1.0.0",
            "[1.0.0]",
            "[OK]"
        );
        
        yield return new TestCaseData(
            "1.6.0",
            "[1.0.0, 2.0.0]",
            "[OK]"
        );
        
        yield return new TestCaseData(
            "2.0.0",
            "[1.0.0, 2.0.0]",
            "[WARN]"
        );
        
        yield return new TestCaseData(
            "2.3.0",
            "[1.0.0, 2.0.0]",
            "[KO]"
        );
        
        yield return new TestCaseData(
            "2.0.0",
            "[1.0.0, 2.0.0)",
            "[KO]"
        );
        
        yield return new TestCaseData(
            "2.3.0",
            "[1.0.0, 3.0.0]",
            "[WARN]"
        );
        
        yield return new TestCaseData(
            "2.3.0",
            "[1.0.0, 3.0.0)",
            "[WARN]"
        );
        
        yield return new TestCaseData(
            "2.3.0",
            "[1.0.0, )",
            "[WARN]"
        );
    }
}