using DangFugBixs.Tests.TestHost;
using NUnit.Framework;

namespace DangFugBixs.Tests;

[TestFixture]
public class GeneratedOutputSnapshotTests {
    [Test]
    public void GeneratedOutput_UsesDocumentedExtensionNamespaceAndSingleScopeInstaller() {
        const string source = """
using NhemDangFugBixs.Attributes;
using VContainer.Unity;
public interface IScopeMarker { }
public interface IGameplayScope : IScopeMarker { }
public interface ICombatCoreService { }
[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemLifetime.Scoped)]
[As(typeof(ICombatCoreService))]
public sealed class CombatCoreService : ICombatCoreService { }
[LifetimeScopeFor(typeof(IGameplayScope))]
public sealed class GameplayLifetimeScope : LifetimeScope { protected override void Configure(global::VContainer.IContainerBuilder builder) { } }
""";

        var (_, generated) = GeneratorTestHost.Run(source);

        Assert.That(generated, Does.Contain("namespace NhemDangFugBixs.VContainer"));
        Assert.That(generated, Does.Contain("public static partial class NhemGeneratedVContainerExtensions"));
        Assert.That(generated, Does.Contain("RegisterGeneratedFor<TScope>(this global::VContainer.IContainerBuilder builder)"));
        Assert.That(generated, Does.Contain("public static partial class NhemGeneratedGameplayScopeInstaller"));
        Assert.That(generated.Split("public static partial class NhemGeneratedGameplayScopeInstaller").Length - 1, Is.EqualTo(1));
    }

    [Test]
    public void GeneratedOutput_DoesNotContainResolvePathsForDeferredCallbacks() {
        const string source = """
using NhemDangFugBixs.Attributes;
using VContainer.Unity;
public interface IScopeMarker { }
public interface IGameplayScope : IScopeMarker { }
[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemLifetime.Scoped)]
public sealed class BuildCallback : IBuildCallback { public void OnBuild(global::VContainer.IObjectResolver resolver) { } }
[LifetimeScopeFor(typeof(IGameplayScope))]
public sealed class GameplayLifetimeScope : LifetimeScope { protected override void Configure(global::VContainer.IContainerBuilder builder) { } }
""";

        var (_, generated) = GeneratorTestHost.Run(source);

        Assert.That(generated, Does.Not.Contain("Resolve<"));
        Assert.That(generated, Does.Not.Contain("RegisterBuildCallback"));
        Assert.That(generated, Does.Not.Contain("RegisterEntryPointExceptionHandler"));
    }

    [Test]
    public void InvalidAsContract_ReportsDiagnosticAndSkipsRegistrationEmission() {
        const string source = """
using NhemDangFugBixs.Attributes;
public interface IScopeMarker { }
public interface IGameplayScope : IScopeMarker { }
public interface ICombatCoreService { }
public interface IOtherContract { }
[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemLifetime.Scoped)]
[As(typeof(IOtherContract))]
public sealed class CombatCoreService : ICombatCoreService { }
[LifetimeScopeFor(typeof(IGameplayScope))]
public sealed class GameplayLifetimeScope { }
""";

        var (result, generated) = GeneratorTestHost.Run(source);

        Assert.That(result.Diagnostics.Any(d => d.Id == "NHEM_DI_001"), Is.True);
        Assert.That(generated, Does.Not.Contain("Register<global::CombatCoreService>"));
    }
}
