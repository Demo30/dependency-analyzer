using DependencyAnalyzer.Library.DependencyExpectationsLoading.Nuspec;
using NUnit.Framework;

namespace Tests;

public class NuspecLoaderTests
{
    [Test]
    public void LoadDepsFromNuspecWithGroupsAndDirectDependencyNodes()
    {
        const string targetFramework = "net9.0";
        const string nuspecContent =
          """
          <?xml version="1.0" encoding="utf-8"?>
          <package xmlns="http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd">
           <metadata>
             <id>Microsoft.Extensions.Logging.Abstractions</id>
             <version>9.0.0</version>
             <authors>Microsoft</authors>
             <license type="expression">MIT</license>
             <licenseUrl>https://licenses.nuget.org/MIT</licenseUrl>
             <icon>Icon.png</icon>
             <readme>PACKAGE.md</readme>
             <projectUrl>https://dot.net/</projectUrl>
             <description>Logging abstractions for Microsoft.Extensions.Logging.

          Commonly Used Types:
          Microsoft.Extensions.Logging.ILogger
          Microsoft.Extensions.Logging.ILoggerFactory
          Microsoft.Extensions.Logging.ILogger&lt;TCategoryName&gt;
          Microsoft.Extensions.Logging.LogLevel
          Microsoft.Extensions.Logging.Logger&lt;T&gt;
          Microsoft.Extensions.Logging.LoggerMessage
          Microsoft.Extensions.Logging.Abstractions.NullLogger</description>
             <releaseNotes>https://go.microsoft.com/fwlink/?LinkID=799421</releaseNotes>
             <copyright>© Microsoft Corporation. All rights reserved.</copyright>
             <serviceable>true</serviceable>
             <repository type="git" url="https://github.com/dotnet/runtime" commit="9d5a6a9aa463d6d10b0b0ba6d5982cc82f363dc3" />
             <dependencies>
               <dependency id="System.Diagnostics.Test1" version="4.5.0" exclude="Build,Analyzers" />
               <group targetFramework=".NETFramework4.6.2">
                 <dependency id="Microsoft.Extensions.DependencyInjection.Abstractions" version="9.0.0" exclude="Build,Analyzers" />
                 <dependency id="System.Diagnostics.DiagnosticSource" version="9.0.0" exclude="Build,Analyzers" />
                 <dependency id="System.Buffers" version="4.5.1" exclude="Build,Analyzers" />
                 <dependency id="System.Memory" version="4.5.5" exclude="Build,Analyzers" />
               </group>
               <dependency id="System.Diagnostics.Test2" version="6.0.2" exclude="Build,Analyzers" />
               <group targetFramework="net8.0">
                 <dependency id="Microsoft.Extensions.DependencyInjection.Abstractions" version="9.0.0" exclude="Build,Analyzers" />
                 <dependency id="System.Diagnostics.DiagnosticSource" version="9.0.0" exclude="Build,Analyzers" />
               </group>
               <group targetFramework="net9.0">
                 <dependency id="Microsoft.Extensions.DependencyInjection.Abstractions" version="9.0.0" exclude="Build,Analyzers" />
               </group>
               <group targetFramework=".NETStandard2.0">
                 <dependency id="Microsoft.Extensions.DependencyInjection.Abstractions" version="9.0.0" exclude="Build,Analyzers" />
                 <dependency id="System.Diagnostics.DiagnosticSource" version="9.0.0" exclude="Build,Analyzers" />
                 <dependency id="System.Buffers" version="4.5.1" exclude="Build,Analyzers" />
                 <dependency id="System.Memory" version="4.5.5" exclude="Build,Analyzers" />
               </group>
               <dependency id="System.Diagnostics.Test3" version="9.2.0" exclude="Build" />
             </dependencies>
           </metadata>
          </package>
          """;
        
        // When
        var deps = NuspecLoader
          .GatherNuspecDependencies(nuspecContent, targetFramework)
          .ToList();

        // Then

        IEnumerable<(string Id, string Version)> expectedDeps =
        [
          ("System.Diagnostics.Test1", "4.5.0"),
          ("System.Diagnostics.Test2", "6.0.2"),
          ("Microsoft.Extensions.DependencyInjection.Abstractions", "9.0.0"),
          ("System.Diagnostics.Test3", "9.2.0")
        ];
        
        Assert.That(deps, Is.EquivalentTo(expectedDeps));
    }
}