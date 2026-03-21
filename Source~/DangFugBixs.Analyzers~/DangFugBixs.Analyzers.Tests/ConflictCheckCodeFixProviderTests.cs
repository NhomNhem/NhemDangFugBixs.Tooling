using System.Threading.Tasks;
using NhemDangFugBixs.Analyzers.CodeFixes;
using NhemDangFugBixs.Analyzers.Rules;
using Xunit;
using Verifier = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.CodeFixVerifier<
    NhemDangFugBixs.Analyzers.Rules.ConflictCheckRule,
    NhemDangFugBixs.Analyzers.CodeFixes.ConflictCheckCodeFixProvider>;

namespace NhemDangFugBixs.Analyzers.Tests;

public class ConflictCheckCodeFixProviderTests {
    [Fact]
    public async Task ND005_RemoveManualRegistration_RemovesInvocationStatement() {
        var test = """
using NhemDangFugBixs.Attributes;
using VContainer;

[AutoRegisterIn(typeof(GameScope))]
public class BulletPresenter { }

public class GameScope : LifetimeScope {
    protected override void Configure(IContainerBuilder builder) {
        {|#0:builder.Register<BulletPresenter>(Lifetime.Scoped)|};
    }
}

namespace NhemDangFugBixs.Attributes {
    public class AutoRegisterInAttribute : System.Attribute {
        public AutoRegisterInAttribute(System.Type scopeType) { }
    }
}

namespace VContainer {
    public enum Lifetime { Singleton, Scoped, Transient }
    public interface IRegistrationBuilder { }
    public interface IContainerBuilder {
        IRegistrationBuilder Register<T>(Lifetime lifetime);
    }
    public class LifetimeScope {
        protected virtual void Configure(IContainerBuilder builder) { }
    }
}
""";

        var fixedCode = """
using NhemDangFugBixs.Attributes;
using VContainer;

[AutoRegisterIn(typeof(GameScope))]
public class BulletPresenter { }

public class GameScope : LifetimeScope {
    protected override void Configure(IContainerBuilder builder) {
    }
}

namespace NhemDangFugBixs.Attributes {
    public class AutoRegisterInAttribute : System.Attribute {
        public AutoRegisterInAttribute(System.Type scopeType) { }
    }
}

namespace VContainer {
    public enum Lifetime { Singleton, Scoped, Transient }
    public interface IRegistrationBuilder { }
    public interface IContainerBuilder {
        IRegistrationBuilder Register<T>(Lifetime lifetime);
    }
    public class LifetimeScope {
        protected virtual void Configure(IContainerBuilder builder) { }
    }
}
""";

        var expected = Verifier.Diagnostic(ConflictCheckRule.ND005Id)
            .WithLocation(0)
            .WithArguments("BulletPresenter", "GameScope");

        await Verifier.VerifyCodeFixAsync(test, expected, fixedCode).ConfigureAwait(false);
    }
}
