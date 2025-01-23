using System.Text.Json;
using DependencyAnalyzer.Library.Analyzer;
using DependencyAnalyzer.Library.DependencyExpectationsLoading.Nuspec;
using DependencyAnalyzer.Library.PackageLoader;

namespace DependencyAnalyzer.Host;

public static class Program
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true
    };
    
    public static void Main(string[] args)
    {
        var nuspecFactory = new NuspecDependencyExpectationsProviderFactory();
        
        Console.WriteLine("Parsing arguments...");
        
        var options = ProgramOptions.ParseArgs(args);
        
        Console.WriteLine($"Loading resolved packages based on '{options.DepsJsonPath}'.");
        
        var resolvedPackagesInfo = PackageLoader.Load(options.DepsJsonPath);
        
        Console.WriteLine($"Performing dependency analysis constrained by target-framework: '{options.TargetFramework}'");
        
        using var analyzer = new Library.Analyzer.DependencyAnalyzer(
            nuspecFactory,
            options,
            resolvedPackagesInfo
        );
        var analysisResult = analyzer.Analyze().Result;
        
        Console.WriteLine($"Dependency analysis completed. Writing results to file: '{options.OutputPath}'");

        WriteResultToFile(analysisResult, options.OutputPath);
    }
    
    private static void WriteResultToFile(AnalysisReport analysisResult, string outputPath)
    {
        if (File.Exists(outputPath))
        {
            File.Delete(outputPath);
        }

        var result = JsonSerializer.Serialize(analysisResult, JsonSerializerOptions);
        
        File.WriteAllText(outputPath, result);
    }

}