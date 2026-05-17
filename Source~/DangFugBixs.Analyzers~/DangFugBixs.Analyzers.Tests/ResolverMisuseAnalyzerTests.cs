using NhemDangFugBixs.Analyzers.Rules;
using NhemDangFugBixs.Analyzers.Tests.TestHost;
using Xunit;

namespace NhemDangFugBixs.Analyzers.Tests;

public class ResolverMisuseAnalyzerTests {
    [Fact]
    public void ServiceInjectingResolver_ReportsNDF052() {
        const string source = """
using NhemDangFugBixs.Attributes;
using VContainer;
[AutoRegisterIn(typeof(GameplayScope), Lifetime = NhemLifetime.Singleton)]
public class BadService {
  public BadService(IObjectResolver resolver) {}
}
public class GameplayScope {}
namespace VContainer { public interface IObjectResolver {} }
namespace NhemDangFugBixs.Attributes {
  public enum NhemLifetime { Singleton, Transient, Scoped }
  public class LifetimeScopeForAttribute : System.Attribute { public LifetimeScopeForAttribute(System.Type t) {} }
  public class AutoRegisterInAttribute : System.Attribute {
    public AutoRegisterInAttribute(System.Type t) {}
    public NhemLifetime Lifetime { get; set; }
  }
}
[NhemDangFugBixs.Attributes.LifetimeScopeFor(typeof(GameplayScope))]
public class GameplayLifetimeScope {}
""";
        var diagnostics = AnalyzerTestHost.Run(source, new ArchitectureGuardrailsRule());
        Assert.Contains(diagnostics, d => d.Id == "NDF052");
    }
}
