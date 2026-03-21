using System.Threading.Tasks;
using NhemDangFugBixs.Analyzers.CodeFixes;
using NhemDangFugBixs.Analyzers.Rules;
using Xunit;
using Verifier = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.CodeFixVerifier<
    NhemDangFugBixs.Analyzers.Rules.DuplicateContractRegistrationRule,
    NhemDangFugBixs.Analyzers.CodeFixes.DuplicateContractRegistrationCodeFixProvider>;

namespace NhemDangFugBixs.Analyzers.Tests;

public class DuplicateContractRegistrationCodeFixProviderTests {
    [Fact]
    public async Task ND112_RemoveDuplicateAttribute_RemovesOneAutoRegisterIn() {
        var test = """
using NhemDangFugBixs.Attributes;

public interface IService { }
public class GameScope { }

[AutoRegisterIn(typeof(GameScope))]
public class {|#0:ServiceA|} : IService { }

[AutoRegisterIn(typeof(GameScope))]
public class ServiceB : IService { }

namespace NhemDangFugBixs.Attributes {
    public class AutoRegisterInAttribute : System.Attribute {
        public AutoRegisterInAttribute(System.Type scopeType) { }
        public bool AsImplementedInterfaces { get; set; } = true;
        public System.Type[] AsTypes { get; set; } = System.Array.Empty<System.Type>();
    }
}
""";

        var fixedCode = """
using NhemDangFugBixs.Attributes;

public interface IService { }
public class GameScope { }

[AutoRegisterIn(typeof(GameScope))]
public class ServiceB : IService { }

namespace NhemDangFugBixs.Attributes {
    public class AutoRegisterInAttribute : System.Attribute {
        public AutoRegisterInAttribute(System.Type scopeType) { }
        public bool AsImplementedInterfaces { get; set; } = true;
        public System.Type[] AsTypes { get; set; } = System.Array.Empty<System.Type>();
    }
}
""";

        var expected = Verifier.Diagnostic(DuplicateContractRegistrationRule.ND112Id)
            .WithLocation(0)
            .WithArguments("IService", "GameScope", "ServiceA, ServiceB");

        await Verifier.VerifyCodeFixAsync(test, expected, fixedCode).ConfigureAwait(false);
    }
}
