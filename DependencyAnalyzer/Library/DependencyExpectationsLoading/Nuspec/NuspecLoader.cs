using System.Xml.Linq;

namespace DependencyAnalyzer.Library.DependencyExpectationsLoading.Nuspec;

public static class NuspecLoader
{
    public static async ValueTask<string?> GetNuspecContent(string pathToNugetCache, string requestingPackagePath)
    {
        var cachedPackageDir = new DirectoryInfo($"{pathToNugetCache}/{requestingPackagePath}");

        if (!cachedPackageDir.Exists)
        {
            Console.WriteLine($"Package {cachedPackageDir.FullName} doesn't exist in the package cache.");
            return null;
        }

        var nuspecCandidate = cachedPackageDir.GetFiles().SingleOrDefault(f => f.Extension == ".nuspec");
        
        if (nuspecCandidate is null)
        {
            Console.WriteLine($"Package {cachedPackageDir.FullName} doesn't contain a nuspec file.");
            return null;
        }

        await using var fs = new FileStream(nuspecCandidate.FullName, FileMode.Open, FileAccess.Read);
        return await new StreamReader(fs).ReadToEndAsync();
    }
    
    public static IEnumerable<(string Id, string Version)> GatherNuspecDependencies(string nuspecContent, string targetFramework)
    {
        var doc = XDocument.Parse(nuspecContent);
        var ns = doc.Root!.GetDefaultNamespace();

        var nodesUnderDependenciesNode = doc.Descendants(ns + "dependencies").Elements();
        var actualDependencyNodes = EnumerateRelevantDependencyNodes(nodesUnderDependenciesNode, ns, targetFramework);
        var dependencies = actualDependencyNodes
            .Select<XElement, (string Id, string Version)?>(d =>
            {
                var id = d.Attribute("id")?.Value;
                var version = d.Attribute("version")?.Value;

                if (id is not null && version is not null)
                {
                    return new ValueTuple<string, string>(id, version);
                }

                Console.WriteLine($"Nuspec dependency node without id: '{id}' or version: '{version}'");
                return null;

            })
            .Cast<(string Id, string Version)>();
        
        return dependencies;
    }

    private static IEnumerable<XElement> EnumerateRelevantDependencyNodes(IEnumerable<XElement> dependencyElement, XNamespace ns, string targetFramework)
    {
        foreach (var dElement in dependencyElement)
        {
            if (dElement.Name.LocalName != "group")
            {
                yield return dElement;
                continue;
            }

            var targetFrameWorkInGroup = dElement.Attribute("targetFramework")?.Value;
            if (targetFrameWorkInGroup != targetFramework)
            {
                continue;
            }
            
            foreach (var child in dElement.Descendants(ns + "dependency"))
            {
                yield return child;
            }
        }
    }
}