using System.Threading.Tasks;
using NhemDangFugBixs.Analyzers.CodeFixes;
using NhemDangFugBixs.Analyzers.Rules;
using Xunit;
using Verifier = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.CodeFixVerifier<
    NhemDangFugBixs.Analyzers.Rules.ScopeReachabilityRule,
    NhemDangFugBixs.Analyzers.CodeFixes.ScopeReachabilityCodeFixProvider>;

namespace NhemDangFugBixs.Analyzers.Tests;

public class ScopeReachabilityCodeFixProviderTests {
    [Fact]
    public async Task ND006_AddLifetimeScopeForBridge_AddsAttributeOnCurrentScope() {
        var test = """
using NhemDangFugBixs.Attributes;

[AutoRegisterIn(typeof(GameScope))]
public class GameplayService {
    public GameplayService(UiService {|#0:ui|}) { }
}

[AutoRegisterIn(typeof(UIScope))]
public class UiService { }

public class GameScope { }
public class UIScope { }

namespace NhemDangFugBixs.Attributes {
    public class AutoRegisterInAttribute : System.Attribute {
        public AutoRegisterInAttribute(System.Type scopeType) { }
    }
    public class LifetimeScopeForAttribute : System.Attribute {
        public LifetimeScopeForAttribute(System.Type identityType) { }
    }
}
""";

        var fixedCode = """
using NhemDangFugBixs.Attributes;

[AutoRegisterIn(typeof(UIScope))]
public class GameplayService {
    public GameplayService(UiService ui) { }
}

[AutoRegisterIn(typeof(UIScope))]
public class UiService { }

public class GameScope { }
public class UIScope { }

namespace NhemDangFugBixs.Attributes {
    public class AutoRegisterInAttribute : System.Attribute {
        public AutoRegisterInAttribute(System.Type scopeType) { }
    }
    public class LifetimeScopeForAttribute : System.Attribute {
        public LifetimeScopeForAttribute(System.Type identityType) { }
    }
}
""";

        var expected = Verifier.Diagnostic(ScopeReachabilityRule.ND006Id)
            .WithLocation(0)
            .WithArguments("GameplayService", "GameScope", "UiService", "UIScope");

        await Verifier.VerifyCodeFixAsync(test, expected, fixedCode).ConfigureAwait(false);
    }
}
