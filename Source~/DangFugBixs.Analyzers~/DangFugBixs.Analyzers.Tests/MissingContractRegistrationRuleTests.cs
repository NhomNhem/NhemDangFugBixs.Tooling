using System.Threading.Tasks;
using Xunit;
using Verifier =
    Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<
        NhemDangFugBixs.Analyzers.Rules.MissingContractRegistrationRule>;

namespace NhemDangFugBixs.Analyzers.Tests;

public class MissingContractRegistrationRuleTests {
    [Fact]
    public async Task ExplicitAsContract_DoesNotReportNd111() {
        var test = """
using NhemDangFugBixs.Attributes;

public interface ICombatCoreService { }

[AutoRegisterIn(typeof(GameScope), AsImplementedInterfaces = false, AsSelf = false)]
[As(typeof(ICombatCoreService))]
public sealed class CombatCoreService : ICombatCoreService { }

public class GameScope { }

namespace NhemDangFugBixs.Attributes {
    public class AutoRegisterInAttribute : System.Attribute {
        public AutoRegisterInAttribute(System.Type scopeType) { }
        public bool AsImplementedInterfaces { get; set; } = true;
        public bool AsSelf { get; set; } = true;
        public System.Type[] AsTypes { get; set; } = System.Array.Empty<System.Type>();
    }

    public class AsAttribute : System.Attribute {
        public AsAttribute(System.Type contractType) { }
    }
}
""";

        await Verifier.VerifyAnalyzerAsync(test).ConfigureAwait(false);
    }
}
