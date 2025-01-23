using DependencyAnalyzer.Library.PackageLoader;
using NUnit.Framework;

namespace Tests;

public class PackageLoaderTests
{
    [Test]
    public void LoadResolvedPackagesBasedOnDepsJson()
    {
        const string depsJson =
        """
        {
          "runtimeTarget": {
            "name": ".NETCoreApp,Version=v9.0",
            "signature": ""
          },
          "compilationOptions": {},
          "targets": {
            ".NETCoreApp,Version=v9.0": {
              "ServiceConsumer/1.0.0": {
                "dependencies": {
                  "Utility": "1.0.0",
                  "bc9686_DirectLibrary_A": "1.0.0",
                  "bc9686_DirectLibrary_B": "2.0.0"
                },
                "runtime": {
                  "ServiceConsumer.dll": {}
                }
              },
              "bc9686_DirectLibrary_A/1.0.0": {
                "dependencies": {
                  "bc9686_TransitiveDependency_A": "1.0.0"
                },
                "runtime": {
                  "lib/net9.0/bc9686_DirectLibrary_A.dll": {
                    "assemblyVersion": "1.0.0.0",
                    "fileVersion": "1.0.0.0"
                  }
                }
              },
              "bc9686_DirectLibrary_B/2.0.0": {
                "dependencies": {
                  "bc9686_TransitiveDependency_B": "2.0.0"
                },
                "runtime": {
                  "lib/net9.0/bc9686_DirectLibrary_B.dll": {
                    "assemblyVersion": "2.0.0.0",
                    "fileVersion": "2.0.0.0"
                  }
                }
              },
              "bc9686_SecondLevelTransitiveDependency/2.0.0": {
                "runtime": {
                  "lib/net9.0/bc9686_SecondLevelTransitiveDependency.dll": {
                    "assemblyVersion": "2.0.0.0",
                    "fileVersion": "2.0.0.0"
                  }
                }
              },
              "bc9686_TransitiveDependency_A/1.0.0": {
                "dependencies": {
                  "bc9686_SecondLevelTransitiveDependency": "2.0.0"
                },
                "runtime": {
                  "lib/net9.0/bc9686_TransitiveDependency_A.dll": {
                    "assemblyVersion": "1.0.0.0",
                    "fileVersion": "1.0.0.0"
                  }
                }
              },
              "bc9686_TransitiveDependency_B/2.0.0": {
                "dependencies": {
                  "bc9686_SecondLevelTransitiveDependency": "2.0.0"
                },
                "runtime": {
                  "lib/net9.0/bc9686_TransitiveDependency_B.dll": {
                    "assemblyVersion": "2.0.0.0",
                    "fileVersion": "2.0.0.0"
                  }
                }
              },
              "Utility/1.0.0": {
                "dependencies": {
                  "bc9686_TransitiveDependency_A": "1.0.0"
                },
                "runtime": {
                  "Utility.dll": {
                    "assemblyVersion": "1.0.0",
                    "fileVersion": "1.0.0.0"
                  }
                }
              }
            }
          },
          "libraries": {
            "ServiceConsumer/1.0.0": {
              "type": "project",
              "serviceable": false,
              "sha512": ""
            },
            "bc9686_DirectLibrary_A/1.0.0": {
              "type": "package",
              "serviceable": true,
              "sha512": "sha512-Hki2DSf/lTPI6fTi7fh8/opt7xXR6UStATkanVD7b8kwPutjP8RT8BPIKSNVfcAg9VZO4YyBeCOCLsmdzb9bQg==",
              "path": "bc9686_directlibrary_a/1.0.0",
              "hashPath": "bc9686_directlibrary_a.1.0.0.nupkg.sha512"
            },
            "bc9686_DirectLibrary_B/2.0.0": {
              "type": "package",
              "serviceable": true,
              "sha512": "sha512-su12L/l43/SDTwNeKsVHwfRYHOjPeqZC3RtaIEaDW0IXJk+4zzOKMlZ3UPTIrGH4gJmurogoGczkwFmSqyZRyg==",
              "path": "bc9686_directlibrary_b/2.0.0",
              "hashPath": "bc9686_directlibrary_b.2.0.0.nupkg.sha512"
            },
            "bc9686_SecondLevelTransitiveDependency/2.0.0": {
              "type": "package",
              "serviceable": true,
              "sha512": "sha512-6fXgZCOIU67PbZqsmQVU3OoWzgam4UM0K3c0u2aXgeS/07sbZi6LAwBOcjwp5VrJ0AL/Iq/DQpxFKwwVOylHaA==",
              "path": "bc9686_secondleveltransitivedependency/2.0.0",
              "hashPath": "bc9686_secondleveltransitivedependency.2.0.0.nupkg.sha512"
            },
            "bc9686_TransitiveDependency_A/1.0.0": {
              "type": "package",
              "serviceable": true,
              "sha512": "sha512-DKXe85Fkphw15Ct8+F+KogpdmK407PtbJuc+nXtiIStCzopaMq823whWB3Mq36Mr5zabDXTMV3xDPpUgKOYNxw==",
              "path": "bc9686_transitivedependency_a/1.0.0",
              "hashPath": "bc9686_transitivedependency_a.1.0.0.nupkg.sha512"
            },
            "bc9686_TransitiveDependency_B/2.0.0": {
              "type": "package",
              "serviceable": true,
              "sha512": "sha512-Twx8YEfWiHppqjH+OKlDs8Kbvd9RJ44PKyvJtRtUHfClTp6WwrxuYUCQFXhS2o6jNMCFQg7w20h5/wDk16SOIA==",
              "path": "bc9686_transitivedependency_b/2.0.0",
              "hashPath": "bc9686_transitivedependency_b.2.0.0.nupkg.sha512"
            },
            "Utility/1.0.0": {
              "type": "project",
              "serviceable": false,
              "sha512": ""
            }
          }
        }
        """;
        
        // When
        var resolvedPackages = PackageLoader.LoadFromContent(depsJson);
        
        // Then
        var expectedResolvedPackages = new Dictionary<string, ResolvedPackageInfo>
        {
          ["bc9686_DirectLibrary_A"] = new(
            "bc9686_DirectLibrary_A",
            "1.0.0",
            "bc9686_directlibrary_a/1.0.0"
          ),
          ["bc9686_DirectLibrary_B"] = new(
            "bc9686_DirectLibrary_B",
            "2.0.0",
            "bc9686_directlibrary_b/2.0.0"
          ),
          ["bc9686_SecondLevelTransitiveDependency"] = new(
            "bc9686_SecondLevelTransitiveDependency",
            "2.0.0",
            "bc9686_secondleveltransitivedependency/2.0.0"
          ),
          ["bc9686_TransitiveDependency_A"] = new(
            "bc9686_TransitiveDependency_A",
            "1.0.0",
            "bc9686_transitivedependency_a/1.0.0"
          ),
          ["bc9686_TransitiveDependency_B"] = new(
            "bc9686_TransitiveDependency_B",
            "2.0.0",
            "bc9686_transitivedependency_b/2.0.0"
          )
        };
        Assert.That(resolvedPackages, Is.EquivalentTo(expectedResolvedPackages));
    }
}