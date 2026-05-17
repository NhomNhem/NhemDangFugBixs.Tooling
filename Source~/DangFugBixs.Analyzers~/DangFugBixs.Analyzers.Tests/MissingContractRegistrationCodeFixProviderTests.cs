using System.Threading.Tasks;
using NhemDangFugBixs.Analyzers.CodeFixes;
using NhemDangFugBixs.Analyzers.Rules;
using Xunit;
using Verifier = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.CodeFixVerifier<
    NhemDangFugBixs.Analyzers.Rules.MissingContractRegistrationRule,
    NhemDangFugBixs.Analyzers.CodeFixes.MissingContractRegistrationCodeFixProvider>;

namespace NhemDangFugBixs.Analyzers.Tests;

public class MissingContractRegistrationCodeFixProviderTests {
    [Fact]
    public async Task ND111_EnableAsImplementedInterfaces_UpdatesAttributeArgument() {
        var test = """
using NhemDangFugBixs.Attributes;

public interface IMyContract { }
public class GameScope { }

[AutoRegisterIn(typeof(GameScope), AsImplementedInterfaces = false)]
public class {|#0:PlayerService|} : IMyContract { }

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

public interface IMyContract { }
public class GameScope { }

[AutoRegisterIn(typeof(GameScope), AsImplementedInterfaces = true)]
public class PlayerService : IMyContract { }

namespace NhemDangFugBixs.Attributes {
    public class AutoRegisterInAttribute : System.Attribute {
        public AutoRegisterInAttribute(System.Type scopeType) { }
        public bool AsImplementedInterfaces { get; set; } = true;
        public System.Type[] AsTypes { get; set; } = System.Array.Empty<System.Type>();
    }
}
""";

        var expected = Verifier.Diagnostic(MissingContractRegistrationRule.ND111Id)
            .WithLocation(0)
            .WithArguments("PlayerService", "IMyContract");

        await Verifier.VerifyCodeFixAsync(test, expected, fixedCode).ConfigureAwait(false);
    }
}
