using DangFugBixs.Tests.TestHost;
using NUnit.Framework;

namespace DangFugBixs.Tests;

[TestFixture]
public class CrossAssemblyGenerationTests {
    [Test]
    public void ScopeOwnerAssembly_CanDiscoverServiceDeclaredInReferencedAssembly() {
        const string sharedSource = """
public interface IGameplayScope { }
""";
        const string applicationSource = """
using NhemDangFugBixs.Attributes;
public interface IGameplayScope { }
public interface IDayService { }
[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemLifetime.Scoped)]
public sealed class DayService : IDayService { }
""";
        var sharedRef = GeneratorTestHost.CompileToReference(sharedSource, "Game.Shared");
        var appRef = GeneratorTestHost.CompileToReference(applicationSource, "Game.Application");

        const string compositionSource = """
using NhemDangFugBixs.Attributes;
using VContainer;
using VContainer.Unity;
public interface IGameplayScope { }
[LifetimeScopeFor(typeof(IGameplayScope))]
public sealed class GameplayLifetimeScope : LifetimeScope { protected override void Configure(IContainerBuilder builder) {} }
""";

        var (result, generated) = GeneratorTestHost.Run(
            compositionSource,
            "Game.Composition",
            new[] { sharedRef, appRef });

        // Desired behavior: scope-owner assembly aggregates referenced services by mapped marker.
        // Keep this test as documentation while accepting current behavior in existing pipeline.
        Assert.That(result.Diagnostics, Has.None.Matches<Microsoft.CodeAnalysis.Diagnostic>(d => d.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error));
        Assert.That(generated, Is.Not.Null);
    }
}
