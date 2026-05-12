# 03 — Scope Marker Pattern Rules

## Purpose

Scope markers allow services in lower layers to declare their target VContainer scope without referencing the real Unity `LifetimeScope` class.

## Correct pattern

Shared assembly:

```csharp
namespace MyGame.Shared.Composition;

public interface IScopeMarker { }
public interface IProjectScope : IScopeMarker { }
public interface IGameplayScope : IScopeMarker { }
public interface IMainMenuScope : IScopeMarker { }
```

Application assembly:

```csharp
[AutoRegisterIn<IGameplayScope>(Lifetime = NhemLifetime.Scoped)]
[As<IPhaseStateMachine>]
public sealed class PhaseStateMachine : IPhaseStateMachine
{
}
```

Composition assembly:

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

## Dependency direction

Allowed:

```txt
Application    -> Shared
Infrastructure -> Shared
Presentation   -> Shared + Application
Composition    -> Shared + Application + Infrastructure + Presentation
```

Forbidden:

```txt
Application -> Composition
Domain      -> Composition
Shared      -> Composition
```

## Marker type choice

Both marker interfaces and marker classes are valid.

Recommended:

```txt
Project architecture markers: interfaces
Sample/test identity markers: classes
```

Examples:

```csharp
public interface IGameplayScope : IScopeMarker { }
```

or:

```csharp
public sealed class GameplayScopeId { }
```

## Diagnostics required

The analyzer/generator should report:

```txt
MissingScopeMapping
- A service uses [AutoRegisterIn(typeof(IMarker))]
- No [LifetimeScopeFor(typeof(IMarker))] exists

DuplicateScopeMapping
- The same marker maps to multiple LifetimeScopes

InvalidMarkerLayer
- Optional warning if marker is declared in Composition instead of Shared/Abstractions
```

## Do not hard-code presets

Do not make `[GameplayService]` require a hard-coded `GameplayLifetimeScope` type.

If presets exist, they must map through alias/config:

```txt
GameplayService = AutoRegisterInScope("Gameplay") + Scoped
```

and `Gameplay` must resolve to a marker or scope mapping.
