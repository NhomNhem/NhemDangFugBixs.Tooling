using NhemDangFugBixs.Common.Models.DiContractGraph;
using NUnit.Framework;

namespace DangFugBixs.Common.Tests;

[TestFixture]
public sealed class DiContractGraphTests {
    private static readonly DiTypeIdentity GameplayScope = DiTypeIdentity.FromFullName("Game.Shared.IGameplayScope", "Game.Shared");
    private static readonly DiTypeIdentity ProjectScope = DiTypeIdentity.FromFullName("Game.Shared.IProjectScope", "Game.Shared");
    private static readonly DiTypeIdentity DayService = DiTypeIdentity.FromFullName("Game.Feature.DayService", "Game.Feature");
    private static readonly DiTypeIdentity NightService = DiTypeIdentity.FromFullName("Game.Feature.NightService", "Game.Feature");
    private static readonly DiTypeIdentity DayContract = DiTypeIdentity.FromFullName("Game.Feature.IDayService", "Game.Feature");
    private static readonly DiTypeIdentity GameplayRoot = DiTypeIdentity.FromFullName("Game.Bootstrap.GameplayLifetimeScope", "Game.Bootstrap");

    [Test]
    public void ServicesForScope_ReturnsStableOrderingAndResolvesDuplicateImplementations() {
        var graph = new DiContractGraph(
            services: new[] {
                Service(NightService, GameplayScope, "Game.Feature", DiEvidenceSource.ReferencedAssembly),
                Service(DayService, GameplayScope, "Game.Feature", DiEvidenceSource.ReferencedAssembly),
                Service(DayService, GameplayScope, "Game.Bootstrap", DiEvidenceSource.CurrentCompilation),
                Service(DayService, ProjectScope, "Game.Feature", DiEvidenceSource.ReferencedAssembly)
            });

        var services = graph.ServicesForScope(GameplayScope);

        Assert.That(services, Has.Count.EqualTo(2));
        Assert.That(services.Select(service => service.ImplementationType.FullName), Is.EqualTo(new[] {
            "Game.Feature.DayService",
            "Game.Feature.NightService"
        }));
        Assert.That(services[0].Provenance.EvidenceSource, Is.EqualTo(DiEvidenceSource.CurrentCompilation));
    }

    [Test]
    public void GraphQueries_ReturnCompositionRootManualRegistrationsAndGeneratedInstallersByIdentity() {
        var graph = new DiContractGraph(
            compositionRoots: new[] {
                new DiCompositionRoot(GameplayRoot, GameplayScope, Provenance("Game.Bootstrap", DiEvidenceSource.CurrentCompilation), new[] { "RegisterGeneratedFor<IGameplayScope>" }, true)
            },
            manualRegistrations: new[] {
                new DiManualRegistration(DayService, "Register", "GameplayLifetimeScope.cs:12", Provenance("Game.Bootstrap", DiEvidenceSource.CurrentCompilation), DayContract, GameplayScope),
                new DiManualRegistration(NightService, "Register", "GameplayLifetimeScope.cs:13", Provenance("Game.Bootstrap", DiEvidenceSource.CurrentCompilation), scopeMarkerType: GameplayScope)
            },
            generatedInstallers: new[] {
                new DiGeneratedInstaller(ProjectScope, "NhemGeneratedProjectScopeInstaller", true, Provenance("Game.Bootstrap", DiEvidenceSource.CurrentCompilation)),
                new DiGeneratedInstaller(GameplayScope, "NhemGeneratedGameplayScopeInstaller", false, Provenance("Game.Bootstrap", DiEvidenceSource.CurrentCompilation))
            });

        Assert.That(graph.CompositionRootForMarker(GameplayScope)?.LifetimeScopeType.FullName, Is.EqualTo("Game.Bootstrap.GameplayLifetimeScope"));
        Assert.That(graph.ManualRegistrationsForImplementation(DayService).Single().ContractType?.FullName, Is.EqualTo("Game.Feature.IDayService"));
        Assert.That(graph.GeneratedInstallersForScope(ProjectScope).Single().IsNoOp, Is.True);
    }

    [Test]
    public void AssemblyProvenance_PreservesDeclaringAssemblyObservedAssemblyAndReferencePath() {
        var provenance = new DiAssemblyProvenance(
            "Game.Feature",
            "Game.Bootstrap",
            DiEvidenceSource.ReferencedAssembly,
            new[] { "Game.Bootstrap", "Game.Feature", "Game.Shared" });
        var graph = new DiContractGraph(services: new[] {
            new DiServiceRegistration(DayService, new[] { DayContract }, "Scoped", provenance, GameplayScope)
        });

        var service = graph.ServicesForScope(GameplayScope).Single();

        Assert.That(service.Provenance.DeclaringAssembly, Is.EqualTo("Game.Feature"));
        Assert.That(service.Provenance.ObservedAssembly, Is.EqualTo("Game.Bootstrap"));
        Assert.That(service.Provenance.ReferencePath, Is.EqualTo(new[] { "Game.Bootstrap", "Game.Feature", "Game.Shared" }));
        Assert.That(service.Provenance.EvidenceSource, Is.EqualTo(DiEvidenceSource.ReferencedAssembly));
    }

    [Test]
    public void RegressionFixture_ModelsSharedMarkerFeatureServiceAndBootstrapCompositionAssemblies() {
        var graph = AfterimageStyleFixture.Create();

        Assert.That(graph.CompositionRootForMarker(GameplayScope), Is.Not.Null);
        Assert.That(graph.ServicesForScope(GameplayScope).Single().Provenance.ReferencePath, Is.EqualTo(new[] {
            "Game.Bootstrap",
            "Game.Feature",
            "Game.Shared"
        }));
        Assert.That(graph.GeneratedInstallersForScope(ProjectScope).Single().IsNoOp, Is.True);
    }

    [Test]
    public void FactoryFromLegacy_PopulatesGraphWithoutChangingLegacyFacts() {
        var service = new NhemDangFugBixs.Common.Models.ServiceInfo(
            "Game.Feature",
            "DayService",
            "Scoped",
            "Global",
            new[] { "Game.Feature.IDayService" },
            false,
            true,
            true,
            false,
            Array.Empty<string>(),
            false,
            false,
            "Game.Shared.IGameplayScope",
            true,
            metadata: new Dictionary<string, string> {
                ["DeclaringAssembly"] = "Game.Feature"
            });
        var mapping = new NhemDangFugBixs.Common.Models.ScopeMappingInfo(
            "Game.Bootstrap",
            "GameplayLifetimeScope",
            "Game.Shared.IGameplayScope",
            "GameplayLifetimeScope",
            "Gameplay");

        var graph = DiContractGraphFactory.FromLegacy("Game.Bootstrap", new[] { service }, new[] { mapping });
        var legacyGameplayScope = DiTypeIdentity.FromFullName("Game.Shared.IGameplayScope");

        Assert.That(graph.ServicesForScope(legacyGameplayScope).Single().ImplementationType.FullName, Is.EqualTo("Game.Feature.DayService"));
        Assert.That(graph.CompositionRootForMarker(legacyGameplayScope)?.LifetimeScopeType.FullName, Is.EqualTo("Game.Bootstrap.GameplayLifetimeScope"));
        Assert.That(graph.GeneratedInstallersForScope(legacyGameplayScope).Single().IsNoOp, Is.False);
    }

    private static DiServiceRegistration Service(
        DiTypeIdentity implementation,
        DiTypeIdentity scope,
        string declaringAssembly,
        DiEvidenceSource source)
        => new DiServiceRegistration(
            implementation,
            new[] { DayContract },
            "Scoped",
            Provenance(declaringAssembly, source),
            scope);

    private static DiAssemblyProvenance Provenance(string declaringAssembly, DiEvidenceSource source)
        => new DiAssemblyProvenance(declaringAssembly, "Game.Bootstrap", source, new[] { "Game.Bootstrap", declaringAssembly });

    private static class AfterimageStyleFixture {
        public static DiContractGraph Create()
            => new DiContractGraph(
                scopes: new[] {
                    new DiScopeIdentity(GameplayScope, new DiAssemblyProvenance("Game.Shared", "Game.Bootstrap", DiEvidenceSource.ReferencedAssembly, new[] { "Game.Bootstrap", "Game.Shared" }), "Gameplay", GameplayRoot),
                    new DiScopeIdentity(ProjectScope, new DiAssemblyProvenance("Game.Shared", "Game.Bootstrap", DiEvidenceSource.ReferencedAssembly, new[] { "Game.Bootstrap", "Game.Shared" }))
                },
                services: new[] {
                    new DiServiceRegistration(
                        DayService,
                        new[] { DayContract },
                        "Scoped",
                        new DiAssemblyProvenance("Game.Feature", "Game.Bootstrap", DiEvidenceSource.ReferencedAssembly, new[] { "Game.Bootstrap", "Game.Feature", "Game.Shared" }),
                        GameplayScope)
                },
                compositionRoots: new[] {
                    new DiCompositionRoot(GameplayRoot, GameplayScope, new DiAssemblyProvenance("Game.Bootstrap", "Game.Bootstrap", DiEvidenceSource.CurrentCompilation, new[] { "Game.Bootstrap" }), hasGeneratedRegistrationCall: true)
                },
                generatedInstallers: new[] {
                    new DiGeneratedInstaller(GameplayScope, "NhemGeneratedGameplayScopeInstaller", false, new DiAssemblyProvenance("Game.Bootstrap", "Game.Bootstrap", DiEvidenceSource.CurrentCompilation, new[] { "Game.Bootstrap" })),
                    new DiGeneratedInstaller(ProjectScope, "NhemGeneratedProjectScopeInstaller", true, new DiAssemblyProvenance("Game.Bootstrap", "Game.Bootstrap", DiEvidenceSource.CurrentCompilation, new[] { "Game.Bootstrap" }))
                });
    }
}
