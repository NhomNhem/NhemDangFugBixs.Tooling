using DangFugBixs.Tests.TestHost;
using NUnit.Framework;

namespace DangFugBixs.Tests;

[TestFixture]
public class ScopeMarkerGenerationTests {
    [Test]
    public void AutoRegisterIn_WithScopeMarker_GeneratesRegistrationForMappedLifetimeScope() {
        const string source = """
using NhemDangFugBixs.Attributes;
using VContainer;
using VContainer.Unity;

public interface IGameplayScope { }

[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemLifetime.Scoped)]
public sealed class PhaseStateMachine : IPhaseStateMachine { }
public interface IPhaseStateMachine { }

[LifetimeScopeFor(typeof(IGameplayScope))]
public sealed class GameplayLifetimeScope : LifetimeScope {
    protected override void Configure(IContainerBuilder builder) { }
}
""";

        var (_, generated) = GeneratorTestHost.Run(source);
        Assert.That(generated, Does.Contain("PhaseStateMachine"));
        Assert.That(generated, Does.Contain("AsImplementedInterfaces()"));
        Assert.That(generated, Does.Contain("NhemGeneratedGameplayScopeInstaller"));
        Assert.That(generated, Does.Contain("RegisterGeneratedFor<TScope>"));
        Assert.That(generated, Does.Contain("RegisterGeneratedForIGameplayScope"));
        Assert.That(generated, Does.Contain("RegisterGameplay"));
    }

    [Test]
    public void LifetimeScopeFor_WithNoServices_GeneratesNoOpRegistrationRoute() {
        const string source = """
using NhemDangFugBixs.Attributes;
using VContainer;
using VContainer.Unity;

public interface IProjectScope { }

[LifetimeScopeFor(typeof(IProjectScope))]
public sealed class ProjectRootLifetimeScope : LifetimeScope {
    protected override void Configure(IContainerBuilder builder) { }
}
""";

        var (_, generated) = GeneratorTestHost.Run(source);

        Assert.That(generated, Does.Contain("NhemGeneratedProjectScopeInstaller"));
        Assert.That(generated, Does.Contain("if (marker == typeof(global::IProjectScope))"));
        Assert.That(generated, Does.Contain("RegisterGeneratedForIProjectScope"));
        Assert.That(generated, Does.Contain("RegisterProjectRoot"));
        Assert.That(generated, Does.Contain("GeneratedInstallers"));
        Assert.That(generated, Does.Contain("IProjectScope|NhemGeneratedProjectScopeInstaller|True"));
        Assert.That(generated, Does.Contain("## Mapped Scopes"));
    }
}
