namespace DependencyAnalyzer.Host;

public class ProgramOptions
{
    public required string NugetPackageCacheDirPath { get; set; }
    
    public required string DepsJsonPath { get; set; }
    
    public required string OutputPath { get; set; }
    
    public required string TargetFramework { get; set; }

    public static ProgramOptions ParseArgs(string[] args)
    {
        var commands = args.Select(a => a.Split("="));

        var nugetCache = "";
        var depsJsonPath = "";
        var outputPath = "";
        var targetFramework = "";
        
        foreach (var command in commands)
        {
            switch (command[0])
            {
                case "nuget-cache":
                    nugetCache = command[1];
                    break;
                case "depsjson":
                    depsJsonPath = command[1];
                    break;
                case "output":
                    outputPath = command[1];
                    break;
                case "target-framework":
                    targetFramework = command[1];
                    break;
            }
        }

        if (string.IsNullOrEmpty(nugetCache) || string.IsNullOrEmpty(depsJsonPath) || string.IsNullOrEmpty(outputPath))
        {
            throw new ArgumentException("Setup all necessary arguments");
        }

        return new ProgramOptions
        {
            NugetPackageCacheDirPath = nugetCache,
            DepsJsonPath = depsJsonPath,
            OutputPath = outputPath,
            TargetFramework = targetFramework
        };
    }
}