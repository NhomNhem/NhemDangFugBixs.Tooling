using NhemDangFugBixs.Analyzers.Rules;
using NhemDangFugBixs.Analyzers.Tests.TestHost;
using Xunit;

namespace NhemDangFugBixs.Analyzers.Tests;

public class InjectionStyleAnalyzerTests {
    [Fact]
    public void PublicInjectField_ReportsNDF021() {
        const string source = """
namespace VContainer { public class InjectAttribute : System.Attribute {} }
namespace NhemDangFugBixs.Attributes { public class AutoRegisterInAttribute : System.Attribute { public AutoRegisterInAttribute(System.Type t) {} } }
[NhemDangFugBixs.Attributes.AutoRegisterIn(typeof(object))]
public class PlayerView {
  [VContainer.Inject] public int Service;
}
""";
        var diagnostics = AnalyzerTestHost.Run(source, new ArchitectureGuardrailsRule());
        Assert.Contains(diagnostics, d => d.Id == "NDF021");
    }
}
