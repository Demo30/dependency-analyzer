namespace DependencyAnalyzer.Library.DependencyExpectationsLoading;

public class DependencyExpectationsResult
{
    public enum ProvisionResult
    {
        Success = 1,
        Failure = 2
    }
    
    public required ProvisionResult Result { get; init; }
    
    public required IEnumerable<(string packageId, string version)> ExpectedDependencies { get; init; }
    
}