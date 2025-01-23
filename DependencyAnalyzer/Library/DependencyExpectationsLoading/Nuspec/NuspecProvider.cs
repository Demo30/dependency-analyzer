namespace DependencyAnalyzer.Library.DependencyExpectationsLoading.Nuspec;

public class NuspecProvider : IDependencyExpectationsProvider
{
    public string ProviderName => "Nuspec source";

    public async ValueTask<DependencyExpectationsResult> ProvideDependencyExpectations(IProviderData providerData)
    {
        if (providerData is not NuspecProviderData nuspecProviderData)
        {
            throw new ArgumentException($"Provided data is not of type {nameof(NuspecProviderData)}");
        }

        var nuspecContent = await NuspecLoader.GetNuspecContent(nuspecProviderData.PathToNugetCache, nuspecProviderData.RequestingPackagePath);

        if (nuspecContent is null)
        {
            return new DependencyExpectationsResult
            {
                Result = DependencyExpectationsResult.ProvisionResult.Failure,
                ExpectedDependencies = []
            };
        }
        
        var dependencies = NuspecLoader.GatherNuspecDependencies(nuspecContent, nuspecProviderData.TargetFramework);

        return new DependencyExpectationsResult
        {
            Result = DependencyExpectationsResult.ProvisionResult.Success,
            ExpectedDependencies = dependencies
        };
    }
}