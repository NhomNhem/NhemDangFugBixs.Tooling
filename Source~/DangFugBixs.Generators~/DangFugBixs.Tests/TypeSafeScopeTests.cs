using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NhemDangFugBixs.Generators;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace DangFugBixs.Tests;

[TestFixture]
public class TypeSafeScopeTests {
    private static readonly MetadataReference RuntimeAssembly = MetadataReference.CreateFromFile(
        typeof(NhemDangFugBixs.Attributes.AutoRegisterInAttribute).Assembly.Location);

    private static readonly MetadataReference VContainerAssembly = MetadataReference.CreateFromFile(
        typeof(VContainer.Lifetime).Assembly.Location);

    [Test]
    public void AutoRegisterIn_GenericAttribute_GeneratesRegistration() {
        // Arrange
        var source = @"
using NhemDangFugBixs.Attributes;
using VContainer.Unity;

public class TestLifetimeScope : LifetimeScope { }

[AutoRegisterIn(typeof(TestLifetimeScope))]
public class TestService { }
";

        // Act
        var result = RunGenerator(source);

        // Assert
        Assert.That(result.GeneratedTrees.Length, Is.GreaterThan(0));
        var generatedCode = result.GeneratedTrees[0].ToString();
        Assert.That(generatedCode, Does.Contain("RegisterTest"));
        Assert.That(generatedCode, Does.Contain("TestService"));
    }

    [Test]
    public void AutoRegisterIn_WithLifetimeParameter_GeneratesCorrectLifetime() {
        // Arrange
        var source = @"
using NhemDangFugBixs.Attributes;
using VContainer.Unity;

public class GameplayLifetimeScope : LifetimeScope { }

[AutoRegisterIn(typeof(GameplayLifetimeScope), Lifetime = Lifetime.Scoped)]
public class EnemySpawner { }
";

        // Act
        var result = RunGenerator(source);

        // Assert
        var generatedCode = result.GeneratedTrees[0].ToString();
        Assert.That(generatedCode, Does.Contain("RegisterGameplay"));
        Assert.That(generatedCode, Does.Contain("Lifetime.Scoped"));
    }

    [Test]
    public void AutoRegisterIn_ConventionBasedNaming_StripsLifetimeScopeSuffix() {
        // Arrange
        var source = @"
using NhemDangFugBixs.Attributes;
using VContainer.Unity;

public class GameplayLifetimeScope : LifetimeScope { }
public class GameLifetimeScope : LifetimeScope { }

[AutoRegisterIn(typeof(GameplayLifetimeScope))]
public class EnemySpawner { }

[AutoRegisterIn(typeof(GameLifetimeScope))]
public class GameService { }
";

        // Act
        var result = RunGenerator(source);

        // Assert
        var generatedCode = result.GeneratedTrees[0].ToString();
        Assert.That(generatedCode, Does.Contain("RegisterGameplay"));
        Assert.That(generatedCode, Does.Contain("RegisterGame"));
        Assert.That(generatedCode, Does.Not.Contain("RegisterGameplayLifetimeScope"));
        Assert.That(generatedCode, Does.Not.Contain("RegisterGameLifetimeScope"));
    }

    [Test]
    public void AutoRegisterIn_WithAsImplementedInterfaces_BindsInterfaces() {
        // Arrange
        var source = @"
using NhemDangFugBixs.Attributes;
using VContainer.Unity;

public interface ITestService { }

public class TestLifetimeScope : LifetimeScope { }

[AutoRegisterIn(typeof(TestLifetimeScope), AsImplementedInterfaces = true)]
public class TestService : ITestService { }
";

        // Act
        var result = RunGenerator(source);

        // Assert
        var generatedCode = result.GeneratedTrees[0].ToString();
        Assert.That(generatedCode, Does.Contain("AsImplementedInterfaces()"));
    }

    [Test]
    public void AutoRegisterIn_WithAsSelf_BindsSelf() {
        // Arrange
        var source = @"
using NhemDangFugBixs.Attributes;
using VContainer.Unity;

public class TestLifetimeScope : LifetimeScope { }

[AutoRegisterIn(typeof(TestLifetimeScope), AsSelf = true)]
public class TestService { }
";

        // Act
        var result = RunGenerator(source);

        // Assert
        var generatedCode = result.GeneratedTrees[0].ToString();
        Assert.That(generatedCode, Does.Contain("AsSelf()"));
    }

    [Test]
    public void AutoRegisterIn_WithEntryPoint_DetectsITickable() {
        // Arrange
        var source = @"
using NhemDangFugBixs.Attributes;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope { }

[AutoRegisterIn(typeof(GameLifetimeScope))]
public class TickSystem : ITickable {
    public void Tick() { }
}
";

        // Act
        var result = RunGenerator(source);

        // Assert
        var generatedCode = result.GeneratedTrees[0].ToString();
        Assert.That(generatedCode, Does.Contain("RegisterEntryPoint<TickSystem>"));
    }

    [Test]
    public void AutoRegisterIn_WithEntryPoint_DetectsIInitializable() {
        // Arrange
        var source = @"
using NhemDangFugBixs.Attributes;
using VContainer.Unity;

public class GameplayLifetimeScope : LifetimeScope { }

[AutoRegisterIn(typeof(GameplayLifetimeScope))]
public class InitSystem : IInitializable {
    public void Initialize() { }
}
";

        // Act
        var result = RunGenerator(source);

        // Assert
        var generatedCode = result.GeneratedTrees[0].ToString();
        Assert.That(generatedCode, Does.Contain("RegisterEntryPoint<InitSystem>"));
    }

    [Test]
    public void AutoRegisterIn_WithMonoBehaviour_GeneratesComponentRegistration() {
        // Arrange
        var source = @"
using NhemDangFugBixs.Attributes;
using VContainer.Unity;
using UnityEngine;

public class GameLifetimeScope : LifetimeScope { }

[AutoRegisterIn(typeof(GameLifetimeScope))]
public class CameraController : MonoBehaviour { }
";

        // Act
        var result = RunGenerator(source);

        // Assert
        var generatedCode = result.GeneratedTrees[0].ToString();
        Assert.That(generatedCode, Does.Contain("RegisterComponentOnNewGameObject<CameraController>"));
    }

    [Test]
    public void AutoRegisterIn_WithRegisterInHierarchy_GeneratesHierarchyRegistration() {
        // Arrange
        var source = @"
using NhemDangFugBixs.Attributes;
using VContainer.Unity;
using UnityEngine;

public class GameLifetimeScope : LifetimeScope { }

[AutoRegisterIn(typeof(GameLifetimeScope), RegisterInHierarchy = true)]
public class AudioManager : MonoBehaviour { }
";

        // Act
        var result = RunGenerator(source);

        // Assert
        var generatedCode = result.GeneratedTrees[0].ToString();
        Assert.That(generatedCode, Does.Contain("RegisterComponentInHierarchy<AudioManager>"));
    }

    [Test]
    public void AutoRegisterIn_MultipleScopes_GeneratesMultipleMethods() {
        // Arrange
        var source = @"
using NhemDangFugBixs.Attributes;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope { }
public class GameplayLifetimeScope : LifetimeScope { }
public class UILifetimeScope : LifetimeScope { }

[AutoRegisterIn(typeof(GameLifetimeScope))]
public class GameService { }

[AutoRegisterIn(typeof(GameplayLifetimeScope))]
public class EnemySpawner { }

[AutoRegisterIn(typeof(UILifetimeScope))]
public class UIService { }
";

        // Act
        var result = RunGenerator(source);

        // Assert
        var generatedCode = result.GeneratedTrees[0].ToString();
        Assert.That(generatedCode, Does.Contain("RegisterGame("));
        Assert.That(generatedCode, Does.Contain("RegisterGameplay("));
        Assert.That(generatedCode, Does.Contain("RegisterUI("));
    }

    [Test]
    public void AutoRegister_LegacyAttribute_ProducesObsoleteWarning() {
        // Arrange
        var source = @"
using NhemDangFugBixs.Attributes;

[AutoRegister(scope: ""Global"")]
public class LegacyService { }
";

        // Act
        var result = RunGenerator(source);

        // Assert
        Assert.That(result.Diagnostics.Any(d => d.Severity == DiagnosticSeverity.Warning && d.Id == "CS0618"), Is.True);
    }

    private GeneratorDriverRunResult RunGenerator(string source) {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        
        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(x => !x.IsDynamic && !string.IsNullOrWhiteSpace(x.Location))
            .Select(x => MetadataReference.CreateFromFile(x.Location))
            .Cast<MetadataReference>()
            .ToList();

        references.Add(RuntimeAssembly);

        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var generator = new VContainerAutoRegisterGenerator();

        var driver = CSharpGeneratorDriver.Create(generator);
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        return driver.GetRunResult();
    }
}

