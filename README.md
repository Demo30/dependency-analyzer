# Motivation
- Many libraries use loose version constraints in their dependency definitions (such as "1.0.0" translating to ">= 1.0.0") and when such dependencies happen to be involved in the diamond dependency situation, this may result in silent version upgrade of the transitive dependency.
- Such an upgrade may very realistically result in binary incompatibility which may manifest in the devilish manner by crashing only in runtime, only during specific method call or even when special execution path is taken.
  - see locally reproducible scenario [in the related repository](https://github.com/Demo30/deps-in-dotnet/tree/scenario/bc9686)
- This tool attempts to give some more clarity (and therefore control) regarding what package version was resolved against which version constraint and provide the developer with warnings in cases of suspicious or potentially problematic version resolutions.

# Implementation
- Somewhat naive approach is chosen.
  - The project needs to be built first (we focus on runtime issues only) and a file "myProject.deps.json" is expected to be generated along with the build.
  - The deps.json file contains info about what packages and versions made it to the actual build.
  - The analyzer goes through the list of resolved packages and attempts to compare these with the constraints defined by each of these packages.
    - The constraints are retrieved from .nuspec files of each of the dependencies and these are retrieved from the local nuget package cache on the local filesystem path.
 
# Example of tools functionality
- We have a Main project, Direct dependency A, Direct dependency B and these both reference Second level dependency
- Direct dependency A uses loose version contraint "1.0.0" and the Direct dependency B requests version "2.0.0" of the Second level dependency in its version constraint.
- This results in Second level dependency being resolved to "2.0.0" and the Direct dependency A is now using this version of the package.
- But since Second level dependency is backward incompatible between versions, this results in runtime crash for Direct dependency A calls (or just some of them).
- The tool analysis result issues a warning that the Second level dependency was resolved to a different version then the precise version constriant and that the final version differs by a major version number.

## Sample result

```
  {
    "AnalyzedPackages": [
        {
        "Id": "bc9686_TransitiveDependency_A",
        "Version": "1.0.0",
        "VersionExpectedByParent": null,
        "AnalysisResult": "[OK] No expectation",
        "ChildrenSourceInfo": "Children loaded from source (Nuspec source).",
        "Children": [
          {
            "Id": "bc9686_SecondLevelTransitiveDependency",
            "Version": "2.0.0",
            "VersionExpectedByParent": "1.0.0",
            "AnalysisResult": "[WARN] Resolved version: 2.0.0 is within expectation [1.0.0,) but differs in major version.",
            "ChildrenSourceInfo": "Max depth reached",
            "Children": []
          }
        ]
      },
    ]
  }
```

# Usage
- Following arguments need to be supplied to the console executable:
  - nuget-cache=C:\Users\some_user\.nuget\packages
    - where packages are cached after nuget restore
  - depsjson="C:\repositories\myRepo\myProject\bin\....\MyProject.deps.json"
    - path to the generated deps.json file containing list of the resolved packages
  - target-framework=net9.0
    - framework contraint used in .nuspec to load matching dependency definitions
  - output=C:\MyResult.json
    - where the resulting analysis report (json file) will get generated

