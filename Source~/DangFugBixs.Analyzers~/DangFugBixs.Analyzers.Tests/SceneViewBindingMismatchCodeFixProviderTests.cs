using System.Threading.Tasks;
using NhemDangFugBixs.Analyzers.CodeFixes;
using NhemDangFugBixs.Analyzers.Rules;
using Xunit;
using Verifier = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.CodeFixVerifier<
    NhemDangFugBixs.Analyzers.Rules.SceneViewBindingMismatchRule,
    NhemDangFugBixs.Analyzers.CodeFixes.SceneViewBindingMismatchCodeFixProvider>;

namespace NhemDangFugBixs.Analyzers.Tests;

public class SceneViewBindingMismatchCodeFixProviderTests {
    [Fact]
    public async Task ND113_AddAutoRegisterInToView_AddsScopeAttributeWithHierarchyFlag() {
        var test = """
using NhemDangFugBixs.Attributes;
using UnityEngine;

public interface IHealthView { }
public class GameScope { }
public class UIScope { }

[AutoRegisterIn(typeof(UIScope))]
public class HealthView : MonoBehaviour, IHealthView { }

[AutoRegisterIn(typeof(GameScope))]
public class HealthPresenter {
    public HealthPresenter(IHealthView {|#0:view|}) { }
}

namespace UnityEngine {
    public class Component { }
    public class MonoBehaviour : Component { }
}

namespace NhemDangFugBixs.Attributes {
    public class AutoRegisterInAttribute : System.Attribute {
        public AutoRegisterInAttribute(System.Type scopeType) { }
        public bool RegisterInHierarchy { get; set; }
    }
}
""";

        var fixedCode = """
using NhemDangFugBixs.Attributes;
using UnityEngine;

public interface IHealthView { }
public class GameScope { }
public class UIScope { }

[AutoRegisterIn(typeof(GameScope), RegisterInHierarchy = true)]
public class HealthView : MonoBehaviour, IHealthView { }

[AutoRegisterIn(typeof(GameScope))]
public class HealthPresenter {
    public HealthPresenter(IHealthView view) { }
}

namespace UnityEngine {
    public class Component { }
    public class MonoBehaviour : Component { }
}

namespace NhemDangFugBixs.Attributes {
    public class AutoRegisterInAttribute : System.Attribute {
        public AutoRegisterInAttribute(System.Type scopeType) { }
        public bool RegisterInHierarchy { get; set; }
    }
}
""";

        var expected = Verifier.Diagnostic(SceneViewBindingMismatchRule.ND113Id)
            .WithLocation(0)
            .WithArguments("HealthPresenter", "IHealthView", "GameScope");

        await Verifier.VerifyCodeFixAsync(test, expected, fixedCode).ConfigureAwait(false);
    }
}
