using DependencyAnalyzer.Host;
using DependencyAnalyzer.Library.PackageLoader;

namespace DependencyAnalyzer.Library.DependencyExpectationsLoading;

public interface IDependencyExpectationsProviderFactory
{
    public IDependencyExpectationsProvider Create();
    
    public IProviderData CreateProviderData(ProgramOptions options, ResolvedPackageInfo resolvedPackageInfo);
}