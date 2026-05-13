# NhemDangFugBixs.Tooling

A compile-time VContainer workflow toolkit for Unity projects that improves developer workflow around dependency injection by providing source-generated registrations, compile-time analyzers, and architecture-aware tooling.

## Overview

`NhemDangFugBixs.Tooling` is a Unity package that makes VContainer registration safer, faster, and more architecture-aware across assembly definition boundaries. It eliminates manual registration boilerplate while providing compile-time checks for dependency injection architecture mistakes.

### Key Benefits

- **Source-Generated Registrations**: Automatically generates VContainer registration code from attributes
- **Compile-Time Safety**: Analyzers detect DI mistakes before runtime
- **Architecture Awareness**: Supports layered Unity architectures with asmdef boundaries
- **AI-Friendly**: Generates documentation that helps AI coding agents understand project architecture
- **Tooling Integration**: CLI and Editor window for validation, reports, and workflow automation

## Installation

### Via Unity Package Manager

1. Open Window > Package Manager in Unity
2. Click the + button > Add package from git URL
3. Enter: `https://github.com/yourusername/NhemDangFugBixs.Tooling.git`
4. Click Add

### Manual Installation

1. Copy the package folder to your Unity project's `Packages` directory
2. Ensure the folder is named `com.nhemdangfugbixs.tooling`
3. Unity will automatically detect and import the package

## Core Concepts

### Scope Marker Pattern

The scope marker pattern allows services to declare their registration intent without creating unwanted dependencies between layers.

Instead of directly referencing concrete `LifetimeScope` types (which creates bad dependency direction), services depend on marker interfaces:

```csharp
// In Shared assembly
namespace SolarPhobia.Shared.Composition;
public interface IScopeMarker { }
public interface IGameplayScope : IScopeMarker { }
```

Services in lower layers reference only the marker:

```csharp
// In Application assembly
[AutoRegisterIn<IGameplayScope>(Lifetime = NhemLifetime.Scoped)]
[As<IDayPhaseMechanicsService>]
public sealed class DayPhaseMechanicsService : IDayPhaseMechanicsService
{
    // Constructor dependencies...
}
```

Composition scopes map markers to real implementations:

```csharp
// In Composition assembly
[LifetimeScopeFor<IGameplayScope>]
public sealed class GameplayLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterGeneratedFor<IGameplayScope>();
    }
}
```

This maintains good dependency direction:
```
Application -> Shared
Composition -> Shared
Composition -> Application
```

### Attribute System

The package uses attributes to declare registration intent:

#### Registration Attributes
- `[AutoRegisterIn]` - Primary registration attribute
- `[AutoRegisterIn<TScope>]` - Generic version
- `[AutoRegisterInScope]` - String-based scope alias
- `[LifetimeScopeFor]` - Maps marker to LifetimeScope
- `[As]` - Specifies service contracts
- `[BindAs]` - Multiple contract binding
- `[AsSelf]` - Register as concrete type
- `[AsImplementedInterfaces]` - Register all interfaces

#### Specialized Attributes
- `[EntryPoint]` / `[AsyncEntryPoint]` - For IStartable/IInitializable services
- `[SceneComponent]` - For existing MonoBehaviour scene objects
- `[NewGameObjectComponent]` - For dynamically created components
- `[Factory]` / `[Spawner]` - For classes needing IObjectResolver
- `[AutoRegisterMessageBrokerIn]` - For MessagePipe event registration
- `[ScriptableConfig]` - For ScriptableObject configuration

### Lifetime Management

```csharp
public enum NhemLifetime
{
    Singleton = 0,
    Transient = 1,
    Scoped = 2
}
```

Lifetime maps directly to VContainer lifetime options.

## Usage Examples

### Basic Service Registration

```csharp
[AutoRegisterIn<IGameplayScope>(Lifetime = NhemLifetime.Scoped)]
[As<IPlayerService>]
public sealed class PlayerService : IPlayerService
{
    public PlayerService(ILogger logger, ISaveService saveService)
    {
        // Constructor injection
    }
    
    // Service implementation
}
```

### Entry Point

```csharp
[AutoRegisterIn<IGameplayScope>(Lifetime = NhemLifetime.Scoped)]
[EntryPoint]
public sealed class GameInitializer : IStartable
{
    private readonly IGameManager _gameManager;
    
    public GameInitializer(IGameManager gameManager)
    {
        _gameManager = gameManager;
    }
    
    public void Start()
    {
        _gameManager.InitializeGame();
    }
}
```

### Scene Component

```csharp
[SceneComponent<IGameplayScope>]
[As<IPlayerView>]
public sealed class PlayerView : MonoBehaviour, IPlayerView
{
    [Inject]
    private void Construct(IPlayerPresenter presenter)
    {
        // Constructor injection via [Inject] method
    }
    
    // MonoBehaviour implementation
}
```

### MessagePipe Integration

```csharp
// Event definition
[AutoRegisterMessageBrokerIn<IGameplayScope>]
public readonly struct SoulServedEvent
{
    public int SoulCount { get; }
    
    public SoulServedEvent(int soulCount) => SoulCount = soulCount;
}

// Consumer
public class DayPhaseHud
{
    public DayPhaseHud(ISubscriber<SoulServedEvent> subscriber)
    {
        // Subscription handled automatically
    }
}
```

## Tooling

### CLI Tools

The package includes a `di-smoke` command-line interface for validation and reporting:

```bash
# Validate project for DI issues
di-smoke preflight MyGame.csproj

# Generate dependency graph
di-smoke graph MyGame.csproj --scope IGameplayScope --format mermaid

# Generate markdown report
di-smoke report MyGame.csproj --format markdown --out docs/generated/di-map.md

# List services in a scope
di-smoke list MyGame.csproj --scope IGameplayScope
```

### Unity Editor Window

Access via: Window > Nhem/DI Diagnostics

Features:
- View scope marker mappings
- List services per scope with filtering and search
- Show duplicates and lifetime warnings
- Display entry points and MessagePipe events
- Run preflight validation
- Generate DI reports and dependency graphs
- Open generated registration files
- Copy reports for AI agent consumption

## Configuration

Create a `.nhem-di.json` file at your project root to customize behavior:

```json
{
  "scopeAliases": {
    "Project": "SolarPhobia.Shared.Composition.IProjectScope",
    "Gameplay": "SolarPhobia.Shared.Composition.IGameplayScope",
    "MainMenu": "SolarPhobia.Shared.Composition.IMainMenuScope"
  },
  "servicePresets": {
    "ProjectService": {
      "scope": "Project",
      "lifetime": "Singleton"
    },
    "GameplayService": {
      "scope": "Gameplay",
      "lifetime": "Scoped"
    }
  },
  "rules": {
    "maxConstructorDependencies": 6,
    "maxMonoBehaviourDependencies": 4,
    "forbidPublicFieldInjection": true,
    "forbidMonoBehaviourConstructorInjection": true,
    "warnRuntimeSingleton": true,
    "allowedResolverConsumers": [
      "Factory",
      "Spawner",
      "Bootstrapper"
    ]
  },
  "runtimeNamespaces": [
    ".Gameplay.",
    ".Phase.",
    ".Combat.",
    ".Player.",
    ".Resources.",
    ".Hazards."
  ]
}
```

## Samples

The package includes several samples demonstrating different usage patterns:

- `BasicAutoRegister` - Simple service registration
- `ScopeMarkerArchitecture` - Cross-asmdef scope marker setup
- `MessagePipeIntegration` - MessagePipe event registration and consumption
- `SceneComponents` - MonoBehaviour scene object integration
- `SolarPhobiaStyleArchitecture` - Complete architecture example

## Documentation

For detailed documentation, see the `Documentation~` folder which includes:

- `getting-started.md` - Introduction and setup
- `scope-marker-pattern.md` - Detailed scope marker explanation
- `attributes.md` - Complete attribute reference
- `binding.md` - Advanced binding features
- `entry-points.md` - Entry point lifecycle management
- `scene-components.md` - MonoBehaviour integration
- `messagepipe.md` - MessagePipe support
- `diagnostics.md` - Analyzer rules and code fixes
- `cli.md` - CLI tool reference
- `editor-window.md` - Editor window guide
- `architecture.md` - Design principles and patterns
- `migration-guide.md` - Upgrading between versions
- `troubleshooting.md` - Common issues and solutions

## How It Works

1. **Attributes**: Developers add attributes to declare registration intent
2. **Source Generator**: Scans for attributes during compilation
3. **Analyzer**: Validates usage against architecture rules
4. **Code Generation**: Produces VContainer registration extension methods
5. **Tooling**: Provides CLI and Editor interfaces for validation and reporting

The generated code looks like hand-written VContainer registrations:

```csharp
public static class NhemVContainerGeneratedInstaller
{
    public static void RegisterGeneratedFor<TScopeMarker>(this IContainerBuilder builder)
    {
        if (typeof(TScopeMarker) == typeof(IGameplayScope))
        {
            builder.Register<DayPhaseMechanicsService>(Lifetime.Scoped)
                .As<IDayPhaseMechanicsService>();
                
            builder.RegisterEntryPoint<PhaseFlowEntryPoint>(Lifetime.Scoped);
        }
    }
}
```

## Principles

- **Explicit Over Implicit**: Registration intent must be declared with attributes
- **Layer Independence**: Lower layers don't depend on composition assemblies
- **Generated Code Clarity**: Output resembles normal VContainer code
- **Compile-Time Validation**: Mistakes are caught before entering Play Mode
- **AI Agent Friendly**: Generated documentation helps automated coding assistants
- **Incremental Adoption**: Can be introduced gradually to existing projects

## Contributing

Contributions are welcome! Please see our [Contributing Guidelines](CONTRIBUTING.md) for details.

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Ensure tests pass
5. Submit a pull request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Built upon [VContainer](https://github.com/hadashiA/VContainer) for dependency injection
- Inspired by architectural patterns from various Unity enterprise projects
- Designed to work smoothly with AI coding assistants like GitHub Copilot