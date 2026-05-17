using System.Text.Json;
using DangFugBixs.Tests.TestHost;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;

namespace DangFugBixs.Tests;

[TestFixture]
public class ImportRegressionTests {
    [Test]
    public void PackageManifest_ShouldDeclareVContainerDependency() {
        var packageJson = LoadRepositoryFile("package.json");
        using var document = JsonDocument.Parse(packageJson);

        Assert.That(document.RootElement.TryGetProperty("dependencies", out var dependencies), Is.True,
            "package.json must declare dependencies so Unity can resolve required packages on Git URL import.");

        var hasVContainer = dependencies.EnumerateObject()
            .Any(p => p.Name.Contains("vcontainer", StringComparison.OrdinalIgnoreCase));
        Assert.That(hasVContainer, Is.True, "Expected at least one VContainer dependency entry in package.json dependencies.");
    }

    [Test]
    public void PackageVersion_ShouldMatchLatestChangelogEntry() {
        var packageJson = LoadRepositoryFile("package.json");
        using var document = JsonDocument.Parse(packageJson);
        var packageVersion = document.RootElement.GetProperty("version").GetString();

        var changelog = LoadRepositoryFile("CHANGELOG.md");
        var latestHeader = changelog.Split('\n')
            .Select(line => line.Trim())
            .FirstOrDefault(line => line.StartsWith("## ", StringComparison.Ordinal));

        Assert.That(latestHeader, Is.Not.Null.And.Not.Empty);
        var latestVersion = latestHeader!.Split(' ')[1];

        Assert.That(packageVersion, Is.EqualTo(latestVersion),
            "package.json version should match the latest changelog version to avoid import/version confusion.");
    }

    [Test]
    public void PackageVersion_ShouldMatchGeneratorProjectVersion() {
        var packageJson = LoadRepositoryFile("package.json");
        using var document = JsonDocument.Parse(packageJson);
        var packageVersion = document.RootElement.GetProperty("version").GetString();

        var generatorProject = LoadRepositoryFile(Path.Combine(
            "Source~",
            "DangFugBixs.Generators~",
            "DangFugBixs.Generators",
            "DangFugBixs.Generators.csproj"));

        var versionLine = generatorProject.Split('\n')
            .Select(line => line.Trim())
            .FirstOrDefault(line => line.StartsWith("<Version>", StringComparison.Ordinal));

        Assert.That(versionLine, Is.Not.Null.And.Not.Empty);

        var projectVersion = versionLine!
            .Replace("<Version>", string.Empty, StringComparison.Ordinal)
            .Replace("</Version>", string.Empty, StringComparison.Ordinal)
            .Trim();

        Assert.That(projectVersion, Is.EqualTo(packageVersion),
            "Generator project version should stay aligned with package.json to avoid banner/runtime drift.");
    }

    [Test]
    public void ReadmeScenario_BuilderRegisterGeneratedForScopeMarker_ShouldCompile() {
        const string source = """
using NhemDangFugBixs.Attributes;
using NhemDangFugBixs.VContainer;
using VContainer;
using VContainer.Unity;

public interface IGameplayScope { }

[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemLifetime.Scoped)]
public sealed class CombatCore : ICombatCore { }
public interface ICombatCore { }

[LifetimeScopeFor(typeof(IGameplayScope))]
public sealed class GameplayLifetimeScope : LifetimeScope {
    protected override void Configure(IContainerBuilder builder) {
        builder.RegisterGeneratedFor<IGameplayScope>();
    }
}

namespace VContainer {
    public enum Lifetime { Singleton, Transient, Scoped }

    public interface IRegistrationBuilder {
        IRegistrationBuilder AsImplementedInterfaces();
        IRegistrationBuilder AsSelf();
    }

    public interface IContainerBuilder {
        IRegistrationBuilder Register<T>(Lifetime lifetime);
    }

    public sealed class RegistrationBuilder : IRegistrationBuilder {
        public IRegistrationBuilder AsImplementedInterfaces() => this;
        public IRegistrationBuilder AsSelf() => this;
    }

    public sealed class ContainerBuilder : IContainerBuilder {
        public IRegistrationBuilder Register<T>(Lifetime lifetime) => new RegistrationBuilder();
    }
}

namespace VContainer.Unity {
    public abstract class LifetimeScope {
        protected abstract void Configure(global::VContainer.IContainerBuilder builder);
    }
}
""";

        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        var references = GetDefaultReferences().ToArray();
        var compilation = CSharpCompilation.Create(
            "ReadmeCompileCheck",
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var generator = new NhemDangFugBixs.Generators.VContainerAutoRegisterGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        var run = driver.RunGeneratorsAndUpdateCompilation(compilation, out var updatedCompilation, out _);

        var diagnostics = updatedCompilation.GetDiagnostics()
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .ToArray();

        Assert.That(diagnostics, Is.Empty,
            "README scenario should compile without missing namespace or missing extension method errors.");
    }

    private static string LoadRepositoryFile(string relativePath) {
        var current = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
        while (current != null) {
            var candidate = Path.Combine(current.FullName, relativePath);
            if (File.Exists(candidate)) {
                return File.ReadAllText(candidate);
            }

            current = current.Parent;
        }

        throw new FileNotFoundException($"Unable to locate '{relativePath}' from test directory.");
    }

    private static IEnumerable<MetadataReference> GetDefaultReferences() {
        var assemblies = new[] {
            typeof(object).Assembly,
            typeof(Console).Assembly,
            typeof(Enumerable).Assembly,
            typeof(Attribute).Assembly,
            typeof(NhemDangFugBixs.Attributes.AutoRegisterInAttribute).Assembly
        };

        foreach (var assembly in assemblies.Distinct()) {
            yield return MetadataReference.CreateFromFile(assembly.Location);
        }

        var trustedAssemblies = ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))
            ?.Split(Path.PathSeparator)
            ?? Array.Empty<string>();

        foreach (var assemblyPath in trustedAssemblies) {
            var fileName = Path.GetFileName(assemblyPath);
            if (fileName is "System.Runtime.dll" or "netstandard.dll" or "System.Collections.dll") {
                yield return MetadataReference.CreateFromFile(assemblyPath);
            }
        }
    }
}
