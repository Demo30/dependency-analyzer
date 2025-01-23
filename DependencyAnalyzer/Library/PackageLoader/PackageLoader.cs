using System.Text.Json.Nodes;

namespace DependencyAnalyzer.Library.PackageLoader;

public static class PackageLoader
{
    public static Dictionary<string, ResolvedPackageInfo> Load(string projectDepsJsonFilePath)
    {
        var jsonString = File.ReadAllText(projectDepsJsonFilePath);
        return LoadFromContent(jsonString);
    }
    
    public static Dictionary<string, ResolvedPackageInfo> LoadFromContent(string projectDepsJsonContent)
    {
        var jsonObject = JsonNode.Parse(projectDepsJsonContent);

        if (jsonObject is null)
        {
            throw GetParseException();
        }
        
        var libraries = jsonObject["libraries"]?.AsObject();

        if (libraries is null)
        {
            throw GetParseException();
        }

        var packageInfos = new Dictionary<string, ResolvedPackageInfo>(); 
        
        foreach (var libraryNode in libraries)
        {
            var type = libraryNode.Value?["type"]?.ToString();

            if (type != "package")
            {
                continue;
            }

            var rawKey = libraryNode.Key;
            var rawKeyDelimiterPos = rawKey.IndexOf('/');
            var packageName = rawKey[..rawKeyDelimiterPos];
            var packageVersionString = rawKey[(rawKeyDelimiterPos + 1)..];
            var packagePath = libraryNode.Value?["path"]?.ToString() ?? "n/a";

            packageInfos.Add(packageName, new ResolvedPackageInfo(packageName, packageVersionString, packagePath));
        }

        return packageInfos;
    }

    private static Exception GetParseException()
    {
        return new InvalidOperationException("Failed to parse deps.json content.");
    }
}