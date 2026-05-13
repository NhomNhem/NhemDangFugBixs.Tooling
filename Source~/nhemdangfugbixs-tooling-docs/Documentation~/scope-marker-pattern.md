# Scope Marker Pattern

Learn how to use the scope marker pattern to maintain clean architectural boundaries while using VContainer for dependency injection.

## Problem Statement

In layered Unity projects, services often live in lower-level assembly definitions:

```txt
SolarPhobia.Shared
SolarPhobia.Domain
SolarPhobia.Application
SolarPhobia.Infrastructure
SolarPhobia.Composition
```

If an Application service uses:

```csharp
[AutoRegisterIn(typeof(GameplayLifetimeScope))]
```

then `Application` must reference `Composition`, which creates an undesirable dependency direction:

```txt
Application       -> Composition  (BAD)
```

This violates the principle that higher-level layers should not depend on lower-level layers.

## Solution: Scope Markers

Use marker interfaces or marker classes in a shared assembly to decouple service registration from concrete scope implementations.

### Step 1: Define Marker Interfaces

Create marker interfaces in a shared assembly that all layers can reference without creating bad dependencies:

```csharp
// SolarPhobia.Shared.Composition.cs
namespace SolarPhobia.Shared.Composition;

public interface IScopeMarker { }

public interface IProjectScope : IScopeMarker { }

public interface IGameplayScope : IScopeMarker { }

public interface IMainMenuScope : IScopeMarker { }
```

### Step 2: Reference Markers in Services

Services in any layer can now reference only the marker interfaces:

```csharp
// SolarPhobia.Application.Services.DayPhaseMechanicsService.cs
using NhemDangFugBixs.Tooling.Attributes;

namespace SolarPhobia.Application.Services;

[AutoRegisterIn<IGameplayScope>(Lifetime = NhemLifetime.Scoped)]
[As<IDayPhaseMechanicsService>]
public sealed class DayPhaseMechanicsService : IDayPhaseMechanicsService
{
    // Constructor dependencies...
}
```

Notice that the Application assembly only needs to reference the Shared assembly, not Composition.

### Step 3: Map Markers to LifetimeScopes

In the Composition assembly (or any assembly that references both Shared and the concrete LifetimeScopes), map the markers to actual implementations:

```csharp
// SolarPhobia.Composition.Scopes.GameplayLifetimeScope.cs
using NhemDangFugBixs.Tooling.Attributes;
using VContainer;

namespace SolarPhobia.Composition.Scopes;

[LifetimeScopeFor<IGameplayScope>]
public sealed class GameplayLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterGeneratedFor<IGameplayScope>();
    }
}
```

## Dependency Direction

With the scope marker pattern, you achieve clean dependency direction:

```txt
Application       -> Shared     (GOOD)
Infrastructure    -> Shared     (GOOD)
Composition       -> Shared     (GOOD)
Composition       -> Application (GOOD)
Composition       -> Infrastructure (GOOD)
```

Instead of the problematic:

```txt
Application       -> Composition (BAD)
```

## Implementation Options

### Marker Interfaces (Recommended)

Use marker interfaces for project architecture scopes:

```csharp
public interface IProjectScope : IScopeMarker { }
public interface IGameplayScope : IScopeMarker { }
public interface IMainMenuScope : IScopeMarker { }
```

### Marker Classes

Alternatively, use marker classes for package samples or users who dislike marker interfaces:

```csharp
public class ProjectScopeId : IScopeMarker { }
public class GameplayScopeId : IScopeMarker { }
public class MainMenuScopeId : IScopeMarker { }
```

Both approaches are valid and work identically with the source generator.

## Scope Registration

Services register using the marker types:

```csharp
[AutoRegisterIn<IGameplayScope>(Lifetime = NhemLifetime.Scoped)]
[As<IDayPhaseMechanicsService>]
public sealed class DayPhaseMechanicsService : IDayPhaseMechanicsService
{
}
```

Composition scopes declare which markers they support:

```csharp
[LifetimeScopeFor<IGameplayScope>]
public sealed class GameplayLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterGeneratedFor<IGameplayScope>();
    }
}
```

## Benefits

1. **Clean Architecture**: Eliminates unwanted dependencies between layers
2. **Flexibility**: Easy to change scope implementations without modifying service code
3. **Testability**: Services depend only on abstractions (markers)
4. **AI-Friendly**: Clear registration intent that AI coding agents can understand
5. **Compile-Time Safety**: Analyzers can verify correct scope mappings

## Common Pitfalls

### Don't Reference Concrete Types in Services

```csharp
// AVOID THIS - creates bad dependency direction
[AutoRegisterIn(typeof(GameplayLifetimeScope))] // BAD
public sealed class MyService : IMyService { }
```

### Do Reference Marker Types

```csharp
// DO THIS - clean dependency direction
[AutoRegisterIn<IGameplayScope>] // GOOD
public sealed class MyService : IMyService { }
```

### Don't Place Markers in Composition Assembly

Markers should typically live in Shared or Application.Abstractions assemblies, not Composition:

```txt
GOOD: SolarPhobia.Shared.Composition.IScopeMarker
BAD:  SolarPhobia.Composition.IScopeMarker  (creates circular dependencies)
```

## Advanced Usage

### Scope Aliases

For convenience, you can create string-based aliases that map to marker types:

```csharp
[RegisterScopeAlias("Gameplay")]
[LifetimeScopeFor(typeof(IGameplayScope))]
public sealed class GameplayLifetimeScope : LifetimeScope
{
}
```

Then services can use:

```csharp
[AutoRegisterInScope("Gameplay", Lifetime = NhemLifetime.Scoped)]
public sealed class MyService : IMyService { }
```

### Preset Attributes

Optional convenience attributes that work over aliases:

```csharp
[GameplayService] // Expands to AutoRegisterInScope("Gameplay", Lifetime = Scoped)
[As<IPlayerService>]
public sealed class PlayerService : IPlayerService { }
```

These require corresponding scope aliases to be defined.

## See Also

- [Attributes Reference](attributes.md) - Detailed attribute documentation
- [Architecture Guide](architecture.md) - Broader architectural principles
- [Getting Started](getting-started.md) - Installation and basic usage