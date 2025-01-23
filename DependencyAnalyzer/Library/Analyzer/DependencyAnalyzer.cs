using DependencyAnalyzer.Host;
using DependencyAnalyzer.Library.DependencyExpectationsLoading;
using DependencyAnalyzer.Library.PackageLoader;
using DependencyAnalyzer.Library.VersionStringParsing;

namespace DependencyAnalyzer.Library.Analyzer;
public class DependencyAnalyzer : IDisposable
{
    private readonly IDependencyExpectationsProvider _expectationsProvider;
    private readonly IDependencyExpectationsProviderFactory _dependencyExpectationsProviderFactory;

    private ProgramOptions _options;
    private Dictionary<string, ResolvedPackageInfo> _resolvedPackages;
    private HashSet<AnalysisRequest> _performedAnalysis;
    private const int MaxDepth = 1;

    private record AnalysisRequest(string PackageIdToAnalyze, (string ExpectationPackageIdSource, string VersionExpectation)? Expectation);

    /// <param name="dependencyExpectationsProviderFactory">Strategy to load dependency expectations from given resolved package.</param>
    /// <param name="options">Configuration from the user.</param>
    /// <param name="resolvedPackages">Exact versions of packages actually resolved by the dotnet restore.</param>
    public DependencyAnalyzer(
        IDependencyExpectationsProviderFactory dependencyExpectationsProviderFactory,
        ProgramOptions options,
        Dictionary<string, ResolvedPackageInfo> resolvedPackages
    )
    {
        _dependencyExpectationsProviderFactory = dependencyExpectationsProviderFactory;
        _options = options;
        _resolvedPackages = resolvedPackages;
        _performedAnalysis = [];

        _expectationsProvider = dependencyExpectationsProviderFactory.Create();
    }
    
    public async Task<AnalysisReport> Analyze()
    {
        var analysisReport = new AnalysisReport();
        
        foreach (var package in _resolvedPackages.Values)
        {
            var analyzedPackageInfo = await AnalyzePackageRecursive(new AnalysisRequest(package.Id, null), 0);
            analysisReport.AnalyzedPackages.Add(analyzedPackageInfo);
        }
        
        return analysisReport;
    }

    private async ValueTask<AnalyzedPackageInfo> AnalyzePackageRecursive(
        AnalysisRequest analysisRequest,
        int currentDepth
    )
    {
        // Analysis was requested for a package that was not resolved during dotnet restore. Analysis cannot be performed since we analyze by comparing resolved version.
        if (!_resolvedPackages.TryGetValue(analysisRequest.PackageIdToAnalyze, out var resolvedPackage))
        {
            return new AnalyzedPackageInfo
            {
                Id = analysisRequest.PackageIdToAnalyze,
                Version = "n/a",
                VersionExpectedByParent = analysisRequest.Expectation?.VersionExpectation,
                AnalysisResult = "[KO] Not present among resolved dependencies"
            };    
        }
        
        // Keep record of analysis request, but skip actual analysis since the same analysis request has already been processed and there's nothing new to learn.
        if (!_performedAnalysis.Add(analysisRequest))
        {
            return new AnalyzedPackageInfo
            {
                Id = resolvedPackage.Id,
                Version = resolvedPackage.Version,
                VersionExpectedByParent = analysisRequest.Expectation?.VersionExpectation,
                AnalysisResult = "Already checked"
            };
        }

        var currentPackageAnalysis = new AnalyzedPackageInfo
        {
            Id = resolvedPackage.Id,
            Version = resolvedPackage.Version,
        };
        
        if (analysisRequest.Expectation is not null)
        {
            currentPackageAnalysis.VersionExpectedByParent = analysisRequest.Expectation.Value.VersionExpectation;
            currentPackageAnalysis.AnalysisResult = VersionChecker.PerformCheck(currentPackageAnalysis.Version, analysisRequest.Expectation.Value.VersionExpectation);
        }
        else
        {
            currentPackageAnalysis.AnalysisResult = "[OK] No expectation";
        }
        
        if (currentDepth >= MaxDepth)
        {
            currentPackageAnalysis.ChildrenSourceInfo = "Max depth reached";
            return currentPackageAnalysis;
        }

        var dependencyExpectationsResult = await _expectationsProvider.ProvideDependencyExpectations(_dependencyExpectationsProviderFactory.CreateProviderData(_options, resolvedPackage));
        
        if (dependencyExpectationsResult.Result == DependencyExpectationsResult.ProvisionResult.Failure)
        {
            currentPackageAnalysis.ChildrenSourceInfo = $"Loading children from provider ('{_expectationsProvider.ProviderName}') failed. Potential children not analyzed.";
            return currentPackageAnalysis;
        }

        currentPackageAnalysis.ChildrenSourceInfo = $"Children loaded from source ({_expectationsProvider.ProviderName}).";
        
        var nextDepth = currentDepth + 1;
        
        foreach (var expectedDependency in dependencyExpectationsResult.ExpectedDependencies)
        {
            var analysisResult = await AnalyzePackageRecursive(
                new AnalysisRequest(
                    expectedDependency.packageId,
                    (analysisRequest.PackageIdToAnalyze, expectedDependency.version)
                ),
                nextDepth
            );
            
            currentPackageAnalysis.Children.Add(analysisResult);
        }
        
        return currentPackageAnalysis;
    }

    public void Dispose()
    {
        _options = null!;
        _resolvedPackages = null!;
        _performedAnalysis = null!;
    }
}