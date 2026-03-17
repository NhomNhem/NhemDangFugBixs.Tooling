using System.Threading.Tasks;
using Xunit;
using Verifier =
    Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<
        NhemDangFugBixs.Analyzers.Rules.MessagePipeBrokerGuardRule>;

namespace NhemDangFugBixs.Analyzers.Tests;

public class MessagePipeBrokerGuardRuleTests {
    [Fact]
    public async Task SameScopeBroker_NoDiagnostic() {
        var test = """
using NhemDangFugBixs.Attributes;
using MessagePipe;

[AutoRegisterMessageBrokerIn(typeof(GameLifetimeScope))]
public class PlayerJoined { }

[AutoRegisterIn(typeof(GameLifetimeScope))]
public class PlayerPublisher {
    public PlayerPublisher(IPublisher<PlayerJoined> publisher) { }
}

public class GameLifetimeScope { }

namespace MessagePipe {
    public interface IPublisher<T> { }
    public interface ISubscriber<T> { }
}

namespace NhemDangFugBixs.Attributes {
    public class AutoRegisterInAttribute : System.Attribute {
        public AutoRegisterInAttribute(System.Type scopeType) { }
    }

    public class AutoRegisterMessageBrokerInAttribute : System.Attribute {
        public AutoRegisterMessageBrokerInAttribute(System.Type scopeType) { }
    }
}
""";

        await Verifier.VerifyAnalyzerAsync(test).ConfigureAwait(false);
    }

    [Fact]
    public async Task RootScopeBroker_NoDiagnostic() {
        var test = """
using NhemDangFugBixs.Attributes;
using MessagePipe;

[AutoRegisterMessageBrokerIn(typeof(ProjectLifetimeScope))]
public class PlayerJoined { }

[AutoRegisterIn(typeof(GameplayLifetimeScope))]
public class PlayerSubscriber {
    public PlayerSubscriber(ISubscriber<PlayerJoined> subscriber) { }
}

public class ProjectLifetimeScope { }
public class GameplayLifetimeScope { }

namespace MessagePipe {
    public interface IPublisher<T> { }
    public interface ISubscriber<T> { }
}

namespace NhemDangFugBixs.Attributes {
    public class AutoRegisterInAttribute : System.Attribute {
        public AutoRegisterInAttribute(System.Type scopeType) { }
    }

    public class AutoRegisterMessageBrokerInAttribute : System.Attribute {
        public AutoRegisterMessageBrokerInAttribute(System.Type scopeType) { }
    }
}
""";

        await Verifier.VerifyAnalyzerAsync(test).ConfigureAwait(false);
    }

    [Fact]
    public async Task MissingBroker_ReportsDiagnostic() {
        var test = """
using NhemDangFugBixs.Attributes;
using MessagePipe;

[AutoRegisterIn(typeof(GameplayLifetimeScope))]
public class PlayerSubscriber {
    public PlayerSubscriber(ISubscriber<PlayerJoined> {|#0:subscriber|}) { }
}

public class PlayerJoined { }
public class GameplayLifetimeScope { }

namespace MessagePipe {
    public interface IPublisher<T> { }
    public interface ISubscriber<T> { }
}

namespace NhemDangFugBixs.Attributes {
    public class AutoRegisterInAttribute : System.Attribute {
        public AutoRegisterInAttribute(System.Type scopeType) { }
    }

    public class AutoRegisterMessageBrokerInAttribute : System.Attribute {
        public AutoRegisterMessageBrokerInAttribute(System.Type scopeType) { }
    }
}
""";

        var expected = Verifier.Diagnostic("ND008")
            .WithLocation(0)
            .WithArguments("PlayerSubscriber", "GameplayLifetimeScope", "Subscriber", "PlayerJoined");

        await Verifier.VerifyAnalyzerAsync(test, expected).ConfigureAwait(false);
    }

    [Fact]
    public async Task NonGeneratedConsumer_NoDiagnostic() {
        var test = """
using MessagePipe;

public class PlayerSubscriber {
    public PlayerSubscriber(ISubscriber<PlayerJoined> subscriber) { }
}

public class PlayerJoined { }

namespace MessagePipe {
    public interface IPublisher<T> { }
    public interface ISubscriber<T> { }
}
""";

        await Verifier.VerifyAnalyzerAsync(test).ConfigureAwait(false);
    }
}
