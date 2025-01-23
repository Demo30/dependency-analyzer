namespace DependencyAnalyzer.Library.Analyzer;

public class AnalyzedPackageInfo
{
    public required string Id { get; set; }
    
    public required string Version { get; set; }
    
    public string? VersionExpectedByParent { get; set; }

    public string AnalysisResult { get; set; } = "N/A";
    
    public string ChildrenSourceInfo { get; set; } = "";
    
    public List<AnalyzedPackageInfo> Children { get; set; } = new();
}