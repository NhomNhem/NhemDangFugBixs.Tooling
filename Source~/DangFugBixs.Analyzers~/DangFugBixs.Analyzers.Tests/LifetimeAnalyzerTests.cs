using NhemDangFugBixs.Analyzers.Rules;
using NhemDangFugBixs.Analyzers.Tests.TestHost;
using Xunit;

namespace NhemDangFugBixs.Analyzers.Tests;

public class LifetimeAnalyzerTests {
    [Fact]
    public void SingletonDependingOnScoped_ReportsNDF030() {
        const string source = """
using NhemDangFugBixs.Attributes;
public class ScopedService {}
[AutoRegisterIn(typeof(GameplayScope), Lifetime = NhemLifetime.Scoped)]
public class GameplayScoped : ScopedService {}
[AutoRegisterIn(typeof(ProjectScope), Lifetime = NhemLifetime.Singleton)]
public class ProjectService {
  public ProjectService(GameplayScoped dep) {}
}
public class GameplayScope {}
public class ProjectScope {}
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
[NhemDangFugBixs.Attributes.LifetimeScopeFor(typeof(ProjectScope))]
public class ProjectLifetimeScope {}
""";
        var diagnostics = AnalyzerTestHost.Run(source, new ArchitectureGuardrailsRule());
        Assert.Contains(diagnostics, d => d.Id == "NDF030");
    }
}
