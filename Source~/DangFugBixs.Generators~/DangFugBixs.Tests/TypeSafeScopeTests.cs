using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NhemDangFugBixs.Generators;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using MessagePipe;

namespace DangFugBixs.Tests;

[TestFixture]
public class TypeSafeScopeTests {
    private static readonly MetadataReference RuntimeAssembly = MetadataReference.CreateFromFile(
        typeof(NhemDangFugBixs.Attributes.AutoRegisterInAttribute).Assembly.Location);

    private static readonly MetadataReference VContainerAssembly = MetadataReference.CreateFromFile(
        typeof(VContainer.Lifetime).Assembly.Location);

    private static readonly MetadataReference MessagePipeAssembly = MetadataReference.CreateFromFile(
        typeof(IPublisher<>).Assembly.Location);

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
        Assert.That(generatedCode, Does.Contain("RegisterEntryPoint<global::TickSystem>"));
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
        Assert.That(generatedCode, Does.Contain("RegisterEntryPoint<global::InitSystem>"));
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
        Assert.That(generatedCode, Does.Contain("RegisterComponentOnNewGameObject<global::CameraController>"));
    }

    [Test]
    public void AutoRegisterIn_WithMonoBehaviourEntryPoint_BindsImplementedInterfaces() {
        // Arrange
        var source = @"
using NhemDangFugBixs.Attributes;
using VContainer.Unity;
using UnityEngine;

public class GameLifetimeScope : LifetimeScope { }

[AutoRegisterIn(typeof(GameLifetimeScope), AsImplementedInterfaces = false)]
public class InfiniteChunkManager : MonoBehaviour, ITickable {
    public void Tick() { }
}
";

        // Act
        var result = RunGenerator(source);

        // Assert
        var generatedCode = result.GeneratedTrees[0].ToString();
        Assert.That(generatedCode, Does.Contain("RegisterComponentOnNewGameObject<global::InfiniteChunkManager>"));
        Assert.That(generatedCode, Does.Not.Contain("RegisterEntryPoint<global::InfiniteChunkManager>"));
        Assert.That(generatedCode, Does.Contain(".AsImplementedInterfaces()"));
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
        Assert.That(generatedCode, Does.Contain("RegisterComponentInHierarchy<global::AudioManager>"));
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
    public void AutoRegisterIn_WithScopeNameAlias_PreservesLegacyUiAlias() {
        var source = @"
using NhemDangFugBixs.Attributes;
using VContainer.Unity;

public interface CrossLayerIdentity { }

[ScopeName(""UI"")]
[LifetimeScopeFor(typeof(CrossLayerIdentity))]
public class UserInterfaceLifetimeScope : LifetimeScope { }

[AutoRegisterIn(typeof(UserInterfaceLifetimeScope))]
public class UIService { }
";

        var result = RunGenerator(source);
        var generatedCode = result.GeneratedTrees[0].ToString();

        Assert.That(generatedCode, Does.Contain("RegisterUI("));
        Assert.That(generatedCode, Does.Contain("RegisterGeneratedForCrossLayerIdentity"));
    }

    [Test]
    public void AutoRegisterInScope_WithAlias_MapsToLifetimeScopeRegistration() {
        var source = @"
using NhemDangFugBixs.Attributes;
using VContainer.Unity;

public interface IGameplayScope { }

[RegisterScopeAlias(""Gameplay"")]
[LifetimeScopeFor(typeof(IGameplayScope))]
public class GameplayLifetimeScope : LifetimeScope { }

[AutoRegisterInScope(""Gameplay"", Lifetime = Lifetime.Scoped)]
public class GameplayService { }
";

        var result = RunGenerator(source);
        var generatedCode = result.GeneratedTrees[0].ToString();

        Assert.That(generatedCode, Does.Contain("RegisterGameplay("));
        Assert.That(generatedCode, Does.Contain("builder.Register<global::GameplayService>(global::VContainer.Lifetime.Scoped)"));
    }

    [Test]
    public void LifetimeScopeFor_GeneratesGenericAndNonGenericInstallerEntryPoints() {
        var source = @"
using NhemDangFugBixs.Attributes;
using VContainer.Unity;

public interface IGameplayScope { }

[LifetimeScopeFor(typeof(IGameplayScope))]
public class GameplayLifetimeScope : LifetimeScope { }

[AutoRegisterIn(typeof(IGameplayScope))]
public class GameplayService { }
";

        var result = RunGenerator(source);
        var generatedCode = result.GeneratedTrees[0].ToString();

        Assert.That(generatedCode, Does.Contain("RegisterGeneratedFor<TScopeMarker>"));
        Assert.That(generatedCode, Does.Contain("RegisterGeneratedForIGameplayScope"));
    }

    [Test]
    public void EntryPointAttribute_EnablesEntryPointRegistration() {
        var source = @"
using NhemDangFugBixs.Attributes;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope { }

[AutoRegisterIn(typeof(GameLifetimeScope))]
[EntryPoint]
public class ManualEntryPointService : IStartable { public void Start() { } }
";

        var result = RunGenerator(source);
        var generatedCode = result.GeneratedTrees[0].ToString();

        Assert.That(generatedCode, Does.Contain("RegisterEntryPoint<global::ManualEntryPointService>"));
    }

    [Test]
    public void SceneComponentAttribute_GeneratesHierarchyRegistration() {
        var source = @"
using NhemDangFugBixs.Attributes;
using VContainer.Unity;
using UnityEngine;

public interface IGameplayScope { }
[LifetimeScopeFor(typeof(IGameplayScope))]
public class GameplayLifetimeScope : LifetimeScope { }

[SceneComponent<IGameplayScope>(Lifetime = NhemLifetime.Scoped)]
public class PlayerHud : MonoBehaviour { }
";

        var result = RunGenerator(source);
        var generatedCode = result.GeneratedTrees[0].ToString();

        Assert.That(generatedCode, Does.Contain("RegisterComponentInHierarchy<global::PlayerHud>()"));
    }

    [Test]
    public void NewGameObjectComponentAttribute_GeneratesNamedRegistration() {
        var source = @"
using NhemDangFugBixs.Attributes;
using VContainer.Unity;
using UnityEngine;

public interface IGameplayScope { }
[LifetimeScopeFor(typeof(IGameplayScope))]
public class GameplayLifetimeScope : LifetimeScope { }

[NewGameObjectComponent<IGameplayScope>(name: ""BulletPool"", Lifetime = NhemLifetime.Scoped)]
public class BulletPool : MonoBehaviour { }
";

        var result = RunGenerator(source);
        var generatedCode = result.GeneratedTrees[0].ToString();

        Assert.That(generatedCode, Does.Contain("RegisterComponentOnNewGameObject<global::BulletPool>(global::VContainer.Lifetime.Scoped, \"BulletPool\")"));
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
        var runDriver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        return runDriver.GetRunResult();
    }

    [Test]
    public void AutoRegisterMessageBrokerIn_GeneratesRegisterMessageBroker() {
        // Arrange
        var source = @"
using NhemDangFugBixs.Attributes;
using VContainer.Unity;
using MessagePipe;

public class GameLifetimeScope : LifetimeScope { }

[AutoRegisterMessageBrokerIn(typeof(GameLifetimeScope))]
public class PlayerJoined : IMessage<PlayerJoined> { }

public interface IMessage<T> { }
";

        // Act
        var result = RunGenerator(source, includeMessagePipe: true);

        // Assert
        Assert.That(result.GeneratedTrees.Length, Is.GreaterThan(0));
        var generatedCode = result.GeneratedTrees[0].ToString();
        Assert.That(generatedCode, Does.Contain("RegisterMessageBroker"));
        Assert.That(generatedCode, Does.Contain("PlayerJoined"));
    }

    [Test]
    public void AutoRegisterMessageBrokerIn_MultipleScopes_GeneratesMultipleRegistrations() {
        // Arrange
        var source = @"
using NhemDangFugBixs.Attributes;
using VContainer.Unity;
using MessagePipe;

public class GameLifetimeScope : LifetimeScope { }
public class UiLifetimeScope : LifetimeScope { }

[AutoRegisterMessageBrokerIn(typeof(GameLifetimeScope))]
[AutoRegisterMessageBrokerIn(typeof(UiLifetimeScope))]
public class PlayerJoined { }
";

        // Act
        var result = RunGenerator(source, includeMessagePipe: true);

        // Assert
        var generatedCode = string.Join("\n", result.GeneratedTrees.Select(t => t.ToString()));
        Assert.That(generatedCode, Does.Contain("RegisterMessageBroker<global::PlayerJoined>"));
    }

    [Test]
    public void AutoRegisterMessageBrokerIn_WithMarkerMapping_UsesMappedScopeRegistrationMethod() {
        var source = @"
using NhemDangFugBixs.Attributes;
using VContainer.Unity;
using MessagePipe;

public interface IGameplayScope { }
[LifetimeScopeFor(typeof(IGameplayScope))]
public class GameplayLifetimeScope : LifetimeScope { }

[AutoRegisterMessageBrokerIn(typeof(IGameplayScope))]
public class SoulServedEvent { }
";

        var result = RunGenerator(source, includeMessagePipe: true);
        var generatedCode = string.Join("\n", result.GeneratedTrees.Select(t => t.ToString()));

        Assert.That(generatedCode, Does.Contain("RegisterGameplay("));
        Assert.That(generatedCode, Does.Contain("RegisterMessageBroker<global::SoulServedEvent>"));
    }

    [Test]
    public void GeneratedReport_IncludesScopeMappingsAndServiceKinds() {
        var source = @"
using NhemDangFugBixs.Attributes;
using VContainer;
using VContainer.Unity;
using UnityEngine;

public interface IGameplayScope { }

[RegisterScopeAlias(""Gameplay"")]
[LifetimeScopeFor(typeof(IGameplayScope))]
public class GameplayLifetimeScope : LifetimeScope { }

[AutoRegisterIn(typeof(IGameplayScope))]
public class TickEntry : ITickable { public void Tick() {} }

[SceneComponent<IGameplayScope>]
public class HudView : MonoBehaviour { }

[AutoRegisterIn(typeof(IGameplayScope))]
public class BootstrapInstaller : IVContainerInstaller {
    public void Install(IContainerBuilder builder) { }
}
";

        var result = RunGenerator(source);
        var generatedCode = string.Join("\n", result.GeneratedTrees.Select(t => t.ToString()));

        Assert.That(generatedCode, Does.Contain("ScopeMappings"));
        Assert.That(generatedCode, Does.Contain("IGameplayScope|GameplayLifetimeScope|Gameplay"));
        Assert.That(generatedCode, Does.Contain("|EntryPoint|"));
        Assert.That(generatedCode, Does.Contain("|Component|"));
        Assert.That(generatedCode, Does.Contain("|Installer|"));
    }

    private static GeneratorDriverRunResult RunGenerator(string source, bool includeMessagePipe = false) {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(x => !x.IsDynamic && !string.IsNullOrWhiteSpace(x.Location))
            .Select(x => MetadataReference.CreateFromFile(x.Location))
            .Cast<MetadataReference>()
            .ToList();

        references.Add(RuntimeAssembly);
        references.Add(VContainerAssembly);
        
        if (includeMessagePipe) {
            references.Add(MessagePipeAssembly);
        }

        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var generator = new VContainerAutoRegisterGenerator();

        var driver = CSharpGeneratorDriver.Create(generator);
        var runDriver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        return runDriver.GetRunResult();
    }
}

