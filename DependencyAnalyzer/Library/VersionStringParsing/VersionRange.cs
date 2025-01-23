namespace DependencyAnalyzer.Library.VersionStringParsing;

public class VersionRange
{
    public required bool FromVersionInclusive { get; init; }
    
    public required Version FromVersion { get; init; }
    
    public required bool ToVersionInclusive { get; init; }
    
    public required Version? ToVersion { get; init; }

    public override string ToString()
    {
        return $"{InclusiveBracketLeft(FromVersionInclusive)}{FromVersion},{ToVersion}{InclusiveBracketRight(ToVersionInclusive)}";
    }

    private char InclusiveBracketLeft(bool isInclusive) => isInclusive ? '[' : '(';
    
    private char InclusiveBracketRight(bool isInclusive) => isInclusive ? ']' : ')';
}