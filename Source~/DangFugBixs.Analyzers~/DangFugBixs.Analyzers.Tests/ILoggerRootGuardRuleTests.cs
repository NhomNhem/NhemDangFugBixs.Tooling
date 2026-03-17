using System.Threading.Tasks;
using Xunit;
using Verifier =
    Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<
        NhemDangFugBixs.Analyzers.Rules.ILoggerRootGuardRule>;

namespace NhemDangFugBixs.Analyzers.Tests;

public class ILoggerRootGuardRuleTests {
    [Fact]
    public async Task ValidRootLoggerSetup_NoDiagnostic() {
        var test = """
using NhemDangFugBixs.Attributes;
using Microsoft.Extensions.Logging;
using VContainer;
using VContainer.Unity;

[AutoRegisterIn(typeof(GameplayLifetimeScope))]
public class PlayerService {
    public PlayerService(ILogger<PlayerService> logger) { }
}

public class ProjectLifetimeScope : LifetimeScope {
    protected override void Configure(IContainerBuilder builder) {
        builder.Register<LoggerFactory>(Lifetime.Singleton).As<ILoggerFactory>();
        builder.Register<LoggerAdapter<PlayerService>>(Lifetime.Singleton);
    }
}

public class GameplayLifetimeScope { }

namespace Microsoft.Extensions.Logging {
    public interface ILoggerFactory { }
    public interface ILogger<TCategoryName> { }
    public class LoggerFactory : ILoggerFactory { }
    public class LoggerAdapter<T> : ILogger<T> { }
}

namespace NhemDangFugBixs.Attributes {
    public class AutoRegisterInAttribute : System.Attribute {
        public AutoRegisterInAttribute(System.Type scopeType) { }
    }
}

namespace VContainer {
    public enum Lifetime { Singleton, Scoped, Transient }
    public interface IRegistrationBuilder {
        IRegistrationBuilder As<T>();
    }
    public interface IContainerBuilder {
        IRegistrationBuilder Register<T>(Lifetime lifetime);
    }
}

namespace VContainer.Unity {
    public class LifetimeScope {
        protected virtual void Configure(VContainer.IContainerBuilder builder) { }
    }
}
""";

        await Verifier.VerifyAnalyzerAsync(test).ConfigureAwait(false);
    }

    [Fact]
    public async Task MissingLoggerFactory_ReportsDiagnostic() {
        var test = """
using NhemDangFugBixs.Attributes;
using Microsoft.Extensions.Logging;
using VContainer;
using VContainer.Unity;

[AutoRegisterIn(typeof(GameplayLifetimeScope))]
public class PlayerService {
    public PlayerService(ILogger<PlayerService> {|#0:logger|}) { }
}

public class ProjectLifetimeScope : LifetimeScope {
    protected override void Configure(IContainerBuilder builder) {
        builder.Register<LoggerAdapter<PlayerService>>(Lifetime.Singleton);
    }
}

public class GameplayLifetimeScope { }

namespace Microsoft.Extensions.Logging {
    public interface ILoggerFactory { }
    public interface ILogger<TCategoryName> { }
    public class LoggerFactory : ILoggerFactory { }
    public class LoggerAdapter<T> : ILogger<T> { }
}

namespace NhemDangFugBixs.Attributes {
    public class AutoRegisterInAttribute : System.Attribute {
        public AutoRegisterInAttribute(System.Type scopeType) { }
    }
}

namespace VContainer {
    public enum Lifetime { Singleton, Scoped, Transient }
    public interface IRegistrationBuilder {
        IRegistrationBuilder As<T>();
    }
    public interface IContainerBuilder {
        IRegistrationBuilder Register<T>(Lifetime lifetime);
    }
}

namespace VContainer.Unity {
    public class LifetimeScope {
        protected virtual void Configure(VContainer.IContainerBuilder builder) { }
    }
}
""";

        var expected = Verifier.Diagnostic("ND009")
            .WithLocation(0)
            .WithArguments("PlayerService", "GameplayLifetimeScope", "PlayerService");

        await Verifier.VerifyAnalyzerAsync(test, expected).ConfigureAwait(false);
    }

    [Fact]
    public async Task MissingLoggerAdapter_ReportsDiagnostic() {
        var test = """
using NhemDangFugBixs.Attributes;
using Microsoft.Extensions.Logging;
using VContainer;
using VContainer.Unity;

[AutoRegisterIn(typeof(GameplayLifetimeScope))]
public class PlayerService {
    public PlayerService(ILogger<PlayerService> {|#0:logger|}) { }
}

public class ProjectLifetimeScope : LifetimeScope {
    protected override void Configure(IContainerBuilder builder) {
        builder.Register<LoggerFactory>(Lifetime.Singleton).As<ILoggerFactory>();
    }
}

public class GameplayLifetimeScope { }

namespace Microsoft.Extensions.Logging {
    public interface ILoggerFactory { }
    public interface ILogger<TCategoryName> { }
    public class LoggerFactory : ILoggerFactory { }
    public class LoggerAdapter<T> : ILogger<T> { }
}

namespace NhemDangFugBixs.Attributes {
    public class AutoRegisterInAttribute : System.Attribute {
        public AutoRegisterInAttribute(System.Type scopeType) { }
    }
}

namespace VContainer {
    public enum Lifetime { Singleton, Scoped, Transient }
    public interface IRegistrationBuilder {
        IRegistrationBuilder As<T>();
    }
    public interface IContainerBuilder {
        IRegistrationBuilder Register<T>(Lifetime lifetime);
    }
}

namespace VContainer.Unity {
    public class LifetimeScope {
        protected virtual void Configure(VContainer.IContainerBuilder builder) { }
    }
}
""";

        var expected = Verifier.Diagnostic("ND009")
            .WithLocation(0)
            .WithArguments("PlayerService", "GameplayLifetimeScope", "PlayerService");

        await Verifier.VerifyAnalyzerAsync(test, expected).ConfigureAwait(false);
    }
}
