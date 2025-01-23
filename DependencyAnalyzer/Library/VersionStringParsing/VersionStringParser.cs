namespace DependencyAnalyzer.Library.VersionStringParsing;

public static class VersionStringParser
{
    public static VersionRange Parse(string version)
    {
        // TODO: improve implementation...
        
        version = version.Replace(" ", string.Empty);
        
        var parts = version.Split(',');

        if (parts.Length == 1)
        {
            var singlePart = parts[0];

            if (singlePart.StartsWith("[") && singlePart.EndsWith("]"))
            {
                var inner2 = singlePart.Replace("[", string.Empty).Replace("(", string.Empty).Replace("]", string.Empty).Replace(")", string.Empty);
                var v2 = Version.Parse(inner2);
                return new VersionRange
                {
                    FromVersionInclusive = true,
                    FromVersion = v2,
                    ToVersionInclusive = true,
                    ToVersion = v2
                };
            }
            
            var inner = singlePart.Replace("[", string.Empty).Replace("(", string.Empty).Replace("]", string.Empty).Replace(")", string.Empty);
            var v1 = Version.Parse(inner);
            return new VersionRange
            {
                FromVersionInclusive = true,
                FromVersion = v1,
                ToVersionInclusive = false,
                ToVersion = null
            };
        }
        
        var fromPart = parts[0];
        var toPart = parts[1];
        
        var fromInclusive = fromPart.StartsWith('[');
        var toInclusive = toPart.EndsWith(']');

        var fromCleaned = fromPart.Replace("[", string.Empty).Replace("(", string.Empty);
        var toCleaned = toPart.Replace("]", string.Empty).Replace(")", string.Empty);
        
        return new VersionRange
        {
            FromVersionInclusive = fromInclusive,
            FromVersion = string.IsNullOrEmpty(fromCleaned) ? new Version(0,0,0) : Version.Parse(fromCleaned),
            ToVersionInclusive = toInclusive,
            ToVersion = string.IsNullOrEmpty(toCleaned) ? null : Version.Parse(toCleaned),
        };
    }
}