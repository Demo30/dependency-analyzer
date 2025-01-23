namespace DependencyAnalyzer.Library.DependencyExpectationsLoading;

public interface IDependencyExpectationsProvider
{
    public string ProviderName { get; }
    
    /// <summary>
    /// Provides list of dependencies as expected by another package
    /// </summary>
    public ValueTask<DependencyExpectationsResult> ProvideDependencyExpectations(IProviderData providerData);
}