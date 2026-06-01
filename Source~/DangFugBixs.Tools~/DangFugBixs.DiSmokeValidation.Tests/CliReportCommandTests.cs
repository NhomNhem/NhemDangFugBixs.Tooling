using DangFugBixs.Cli;
using Xunit;

namespace NhemDangFugBixs.DiSmokeValidation.Tests;

public class CliReportCommandTests {
    private static readonly RegistrationReportSnapshot Snapshot = new(
        "fake.dll",
        new[] { "ProjectLifetimeScope", "GameplayLifetimeScope" },
        new[] {
            new RegistrationReportEntry("ProjectLifetimeScope", "Game.ProjectBootstrap", "Singleton", "EntryPoint", string.Empty),
            new RegistrationReportEntry("GameplayLifetimeScope", "Game.PlayerService", "Scoped", "Standard", string.Empty),
            new RegistrationReportEntry("GameplayLifetimeScope", "Game.PlayerJoinedBroker", "Singleton", "MessageBroker", "Game.PlayerJoined")
        },
        new[] {
            new RegistrationReportConsumer("GameplayLifetimeScope", "Game.PlayerHud", "Subscriber", "Game.PlayerJoined")
        },
        new[] {
            new RegistrationLoggerRoot("ProjectLifetimeScope", true, true)
        },
        new[] {
            new RegistrationLoggerConsumer("GameplayLifetimeScope", "Game.PlayerService", "Game.PlayerService")
        },
        new[] {
            new RegistrationGeneratedInstaller("GameplayLifetimeScope", "NhemGeneratedGameplayScopeInstaller", false),
            new RegistrationGeneratedInstaller("ProjectLifetimeScope", "NhemGeneratedProjectScopeInstaller", true)
        },
        new[] {
            new RegistrationGraphEvidence("GameplayLifetimeScope", "Game.PlayerService", "Game.Services", "Game.Composition", "Game.Composition>Game.Services")
        },
        "# Test Markdown");

    [Fact]
    public void WriteList_FiltersToRequestedScope() {
        var output = RegistrationReportWriters.WriteList(Snapshot, "GameplayLifetimeScope");

        Assert.Contains("[GameplayLifetimeScope]", output, StringComparison.Ordinal);
        Assert.Contains("PlayerService", output, StringComparison.Ordinal);
        Assert.DoesNotContain("ProjectBootstrap", output, StringComparison.Ordinal);
    }

    [Fact]
    public void WriteMermaidGraph_IncludesConsumerEdges() {
        var output = RegistrationReportWriters.WriteMermaidGraph(Snapshot, "GameplayLifetimeScope");

        Assert.Contains("flowchart TD", output, StringComparison.Ordinal);
        Assert.Contains("Subscriber", output, StringComparison.Ordinal);
        Assert.Contains("PlayerJoined", output, StringComparison.Ordinal);
    }

    [Fact]
    public void WriteJson_ContainsMachineReadableEntries() {
        var output = RegistrationReportWriters.WriteJson(Snapshot, "GameplayLifetimeScope");

        Assert.Contains("\"scopeFilter\": \"GameplayLifetimeScope\"", output, StringComparison.Ordinal);
        Assert.Contains("\"Service\": \"Game.PlayerService\"", output, StringComparison.Ordinal);
        Assert.Contains("generatedInstallers", output, StringComparison.Ordinal);
        Assert.Contains("graphEvidence", output, StringComparison.Ordinal);
        Assert.Contains("NhemGeneratedProjectScopeInstaller", output, StringComparison.Ordinal);
    }

    [Fact]
    public void WriteMarkdown_UsesGeneratedMarkdownWhenUnfiltered() {
        var output = RegistrationReportWriters.WriteMarkdown(Snapshot);

        Assert.Equal("# Test Markdown", output);
    }
}
