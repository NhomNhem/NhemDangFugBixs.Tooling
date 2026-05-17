# NhemDangFugBixs Tooling Docs

This site documents the Phase 1/2 public API that is currently shipped by `NhemDangFugBixs.VContainer.SourceGenerator`.

## Current documented primitives

- `AutoRegisterIn<TScope>`
- `AutoRegisterIn(typeof(TScope))`
- `As<TContract>`
- `As(typeof(TContract))`
- `AsSelf`
- `LifetimeScopeFor<TScope>`
- `EntryPoint`
- `RegisterComponentInHierarchy`
- `builder.RegisterGeneratedFor<TScope>()`

The generated extension namespace is:

```csharp
using NhemDangFugBixs.VContainer;
```

## Minimal example

```csharp
using NhemDangFugBixs.Attributes;
using NhemDangFugBixs.VContainer;
using VContainer;
using VContainer.Unity;

public interface IScopeMarker { }
public interface IGameplayScope : IScopeMarker { }
public interface ICombatCoreService { }

[AutoRegisterIn<IGameplayScope>(Lifetime = NhemLifetime.Scoped)]
[As<ICombatCoreService>]
public sealed class CombatCoreService : ICombatCoreService { }

[LifetimeScopeFor<IGameplayScope>]
public sealed class GameplayLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterGeneratedFor<IGameplayScope>();
    }
}
```

Generated output:

```csharp
builder.Register<CombatCoreService>(Lifetime.Scoped)
    .As<ICombatCoreService>();
```

## Architecture policy

- Users own `LifetimeScope` classes.
- The generator owns stateless installers and the dispatch extension.
- The package does not generate `LifetimeScope` MonoBehaviours by default.
- The package does not partial-inject into user `LifetimeScope` classes.
