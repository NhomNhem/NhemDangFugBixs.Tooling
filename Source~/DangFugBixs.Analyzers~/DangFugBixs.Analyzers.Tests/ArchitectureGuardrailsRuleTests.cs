using System.Threading.Tasks;
using Xunit;
using Verifier =
    Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<
        NhemDangFugBixs.Analyzers.Rules.ArchitectureGuardrailsRule>;

namespace NhemDangFugBixs.Analyzers.Tests;

public class ArchitectureGuardrailsRuleTests {
    [Fact]
    public async Task MissingScopeMapping_ReportsNDFG010() {
        var test = """
using NhemDangFugBixs.Attributes;
[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemDangFugBixs.Attributes.NhemLifetime.Scoped)]
public class {|#0:MyService|} { }
public interface IGameplayScope { }
namespace NhemDangFugBixs.Attributes {
  public class AutoRegisterInAttribute : System.Attribute {
    public AutoRegisterInAttribute(System.Type t) {}
    public NhemLifetime Lifetime { get; set; }
  }
  public enum NhemLifetime { Singleton, Transient, Scoped }
  public class LifetimeScopeForAttribute : System.Attribute { public LifetimeScopeForAttribute(System.Type t) {} }
}
""";
        var expected = Verifier.Diagnostic("NDFG010").WithLocation(0).WithArguments("MyService", "IGameplayScope");
        await Verifier.VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task DuplicateScopeMapping_ReportsNDFG011() {
        var test = """
using NhemDangFugBixs.Attributes;
public interface IGameplayScope { }
[LifetimeScopeFor(typeof(IGameplayScope))] public class {|#0:A|} { }
[LifetimeScopeFor(typeof(IGameplayScope))] public class {|#1:B|} { }
namespace NhemDangFugBixs.Attributes {
  public class LifetimeScopeForAttribute : System.Attribute { public LifetimeScopeForAttribute(System.Type t) {} }
}
""";
        var e0 = Verifier.Diagnostic("NDFG011").WithLocation(0).WithArguments("IGameplayScope");
        var e1 = Verifier.Diagnostic("NDFG011").WithLocation(1).WithArguments("IGameplayScope");
        await Verifier.VerifyAnalyzerAsync(test, e0, e1);
    }

    [Fact]
    public async Task PrivateInjectMethod_ReportsNDF022() {
        var test = """
using VContainer;

public class MyService {
    [Inject] private void {|#0:Construct|}(IService service) { }
}
public interface IService { }

namespace VContainer { public class InjectAttribute : System.Attribute {} }
""";
        var expected = Verifier.Diagnostic("NDF022").WithLocation(0).WithArguments("Construct");
        await Verifier.VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task PublicInjectMethod_DoesNotWarn() {
        var test = """
using VContainer;

public class MyService {
    [Inject] public void Construct(IService service) { }
}
public interface IService { }

namespace VContainer { public class InjectAttribute : System.Attribute {} }
""";
        await Verifier.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task InjectionAndSubjectRules_ReportDiagnostics() {
        var test = """
using VContainer;
using UnityEngine;
using R3;

[NhemDangFugBixs.Attributes.AutoRegisterIn(typeof(GameScope))]
public class {|#0:PlayerView|} : MonoBehaviour {
  public {|#4:PlayerView|}(int v) { }
  [Inject] public int {|#1:value|};
  [Inject] public async System.Threading.Tasks.Task {|#2:Constructs|}() { await System.Threading.Tasks.Task.CompletedTask; }
  public Subject<int> {|#3:OnValue|} = new();
}
public class GameScope { }
[NhemDangFugBixs.Attributes.LifetimeScopeFor(typeof(GameScope))]
public class GameLifetimeScope {}

namespace VContainer { public class InjectAttribute : System.Attribute {} }
namespace UnityEngine { public class Component {} public class MonoBehaviour : Component {} }
namespace R3 { public class Subject<T> {} }
namespace NhemDangFugBixs.Attributes {
  public class AutoRegisterInAttribute : System.Attribute { public AutoRegisterInAttribute(System.Type t) {} }
  public class LifetimeScopeForAttribute : System.Attribute { public LifetimeScopeForAttribute(System.Type t) {} }
}
""";
        await Verifier.VerifyAnalyzerAsync(
            test,
            Verifier.Diagnostic("NDF020").WithLocation(4).WithArguments("PlayerView"),
            Verifier.Diagnostic("NDF021").WithLocation(1).WithArguments("value"),
            Verifier.Diagnostic("NDF023").WithLocation(2).WithArguments("Constructs"),
            Verifier.Diagnostic("NDF024").WithLocation(2).WithArguments("Constructs"),
            Verifier.Diagnostic("NDF070").WithLocation(3).WithArguments("OnValue"),
            Verifier.Diagnostic("NDF071").WithLocation(0).WithArguments("PlayerView"));
    }
}
