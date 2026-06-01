using NhemDangFugBixs.Analyzers.Rules;
using NhemDangFugBixs.Analyzers.Tests.TestHost;
using Xunit;

namespace NhemDangFugBixs.Analyzers.Tests;

public class ScopeMappingAnalyzerTests {
    [Fact]
    public void UnmappedMarker_ReportsNDFG014() {
        const string source = """
using NhemDangFugBixs.Attributes;
public interface IGameplayScope {}
[AutoRegisterIn(typeof(IGameplayScope))]
public class GameplayService {}
namespace NhemDangFugBixs.Attributes {
  public class AutoRegisterInAttribute : System.Attribute { public AutoRegisterInAttribute(System.Type t) {} }
}
""";
        var diagnostics = AnalyzerTestHost.Run(source, new ArchitectureGuardrailsRule());
        Assert.Contains(diagnostics, d => d.Id == "NDFG014");
    }
}
