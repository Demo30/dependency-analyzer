using DependencyAnalyzer.Host;
using DependencyAnalyzer.Library.PackageLoader;

namespace DependencyAnalyzer.Library.DependencyExpectationsLoading.Nuspec;

public class NuspecDependencyExpectationsProviderFactory : IDependencyExpectationsProviderFactory
{
    public IDependencyExpectationsProvider Create()
    {
        return new NuspecProvider();
    }

    public IProviderData CreateProviderData(ProgramOptions options, ResolvedPackageInfo resolvedPackageInfo)
    {
        return new NuspecProviderData
        {
            PathToNugetCache = options.NugetPackageCacheDirPath,
            RequestingPackagePath = resolvedPackageInfo.Path,
            TargetFramework = options.TargetFramework,
        };
    }
}