using NhemDangFugBixs.Analyzers.Rules;
using NhemDangFugBixs.Analyzers.Tests.TestHost;
using Xunit;

namespace NhemDangFugBixs.Analyzers.Tests;

public sealed class SemanticScopeDependencyAnalyzerTests {
    [Fact]
    public void DifferentKnownScopes_ReportND006() {
        const string source = """
using NhemDangFugBixs.Attributes;

public interface IProjectScope {}
public interface IGameplayScope {}
public interface IProjectService {}

[AutoRegisterIn(typeof(IProjectScope))]
public sealed class ProjectService : IProjectService {}

[AutoRegisterIn(typeof(IGameplayScope))]
public sealed class GameplayService {
    public GameplayService(IProjectService projectService) {}
}

namespace NhemDangFugBixs.Attributes {
  public class AutoRegisterInAttribute : System.Attribute { public AutoRegisterInAttribute(System.Type t) {} }
}
""";

        var diagnostics = AnalyzerTestHost.Run(source, new SemanticScopeDependencyAnalyzer());

        Assert.Contains(diagnostics, diagnostic => diagnostic.Id == "ND006");
    }

    [Fact]
    public void SameKnownScope_DoesNotReportND006() {
        const string source = """
using NhemDangFugBixs.Attributes;

public interface IGameplayScope {}
public interface IGameplayConfig {}

[AutoRegisterIn(typeof(IGameplayScope))]
public sealed class GameplayConfig : IGameplayConfig {}

[AutoRegisterIn(typeof(IGameplayScope))]
public sealed class GameplayService {
    public GameplayService(IGameplayConfig config) {}
}

namespace NhemDangFugBixs.Attributes {
  public class AutoRegisterInAttribute : System.Attribute { public AutoRegisterInAttribute(System.Type t) {} }
}
""";

        var diagnostics = AnalyzerTestHost.Run(source, new SemanticScopeDependencyAnalyzer());

        Assert.DoesNotContain(diagnostics, diagnostic => diagnostic.Id == "ND006");
    }

    [Fact]
    public void DependencyWithoutCurrentCompilationRegistration_DoesNotReportND006() {
        const string source = """
using NhemDangFugBixs.Attributes;

public interface IGameplayScope {}
public interface IProjectService {}

[AutoRegisterIn(typeof(IGameplayScope))]
public sealed class GameplayService {
    public GameplayService(IProjectService projectService) {}
}

namespace NhemDangFugBixs.Attributes {
  public class AutoRegisterInAttribute : System.Attribute { public AutoRegisterInAttribute(System.Type t) {} }
}
""";

        var diagnostics = AnalyzerTestHost.Run(source, new SemanticScopeDependencyAnalyzer());

        Assert.DoesNotContain(diagnostics, diagnostic => diagnostic.Id == "ND006");
    }
}
