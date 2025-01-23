namespace DependencyAnalyzer.Library.DependencyExpectationsLoading.Nuspec;

public class NuspecProviderData : IProviderData
{
    public required string PathToNugetCache { get; init; }
    public required string RequestingPackagePath { get; init; }
    
    public required string TargetFramework { get; init; }
}