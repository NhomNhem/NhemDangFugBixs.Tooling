using System.Threading.Tasks;
using Xunit;
using Verifier =
    Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<
        NhemDangFugBixs.Analyzers.Rules.ConflictCheckRule>;

namespace NhemDangFugBixs.Analyzers.Tests;

public class ConflictCheckTests {
    [Fact]
    public async Task ManualRegistration_OfAutoRegisteredType_ReportsError() {
        var test = @"
using NhemDangFugBixs.Attributes;
using VContainer;

[AutoRegisterIn(typeof(GameScope))]
public class BulletPresenter { }

public class GameScope : LifetimeScope {
    protected override void Configure(IContainerBuilder builder) {
        builder.Register<BulletPresenter>(Lifetime.Scoped);
    }
}

namespace NhemDangFugBixs.Attributes {
    public class AutoRegisterInAttribute : System.Attribute {
        public AutoRegisterInAttribute(System.Type scope) { }
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
";

        var expected = Verifier.Diagnostic("ND005")
            .WithSpan(10, 9, 10, 59)
            .WithArguments("BulletPresenter", "GameScope");

        await Verifier.VerifyAnalyzerAsync(test, expected).ConfigureAwait(false);
    }

    [Fact]
    public async Task SameNamedRegistrationMethod_OnNonVContainerApi_NoDiagnostic() {
        var test = @"
using NhemDangFugBixs.Attributes;
using Demo.Container;

[AutoRegisterIn(typeof(GameScope))]
public class BulletPresenter { }

public class GameScope {
    protected void Configure(IContainerBuilder builder) {
        builder.Register<BulletPresenter>(Lifetime.Scoped);
    }
}

namespace NhemDangFugBixs.Attributes {
    public class AutoRegisterInAttribute : System.Attribute {
        public AutoRegisterInAttribute(System.Type scope) { }
    }
}

namespace Demo.Container {
    public enum Lifetime { Singleton, Scoped, Transient }
    public interface IRegistrationBuilder { }
    public interface IContainerBuilder {
        IRegistrationBuilder Register<T>(Lifetime lifetime);
    }
}
";

        await Verifier.VerifyAnalyzerAsync(test).ConfigureAwait(false);
    }
}
