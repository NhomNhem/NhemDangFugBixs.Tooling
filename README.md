# NhemDangFugBixs.VContainer.SourceGenerator

Compile-time VContainer workflow tooling for Unity.

## Current shipped API surface (Phase 1/2)

This package currently ships and documents these registration primitives:

- `[AutoRegisterIn<TScope>]`
- `[AutoRegisterIn(typeof(TScope))]`
- `[As<TContract>]`
- `[As(typeof(TContract))]`
- `[AsSelf]`
- `[LifetimeScopeFor<TScope>]`
- `[LifetimeScopeFor(typeof(TScope))]`
- `[EntryPoint]`
- `[RegisterComponentInHierarchy]`
- `builder.RegisterGeneratedFor<TScope>()`

The generated extension lives in:

```csharp
using NhemDangFugBixs.VContainer;
```

Core principle:

```txt
NhemDangFugBixs.Tooling does not force your architecture.
It lets your architecture become compile-time checked.
```

## What the generator owns

- One stateless generated installer per scope marker.
- One generated extension dispatcher: `RegisterGeneratedFor<TScope>()`.
- Readable VContainer registration code that stays close to hand-written code.

## What the user owns

- Scope marker interfaces.
- `LifetimeScope` classes.
- Services and contracts.
- Any manual installer exceptions.

The package does not generate Unity `LifetimeScope` MonoBehaviours by default.
It does not partial-inject into your `LifetimeScope` classes.

## Install

`package.json` declares the pinned VContainer dependency used by this package.
If you import through Unity Package Manager from Git or a package feed, Unity should resolve VContainer from package metadata.

## Minimal usage

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
public sealed class CombatCoreService : ICombatCoreService
{
}

[LifetimeScopeFor<IGameplayScope>]
public sealed class GameplayLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterGeneratedFor<IGameplayScope>();
    }
}
```

Generated output shape:

```csharp
builder.Register<CombatCoreService>(Lifetime.Scoped)
    .As<ICombatCoreService>();
```

Generated extension shape:

```csharp
public static void RegisterGeneratedFor<TScope>(this IContainerBuilder builder)
```

## Phase 2 examples

Entry point:

```csharp
[AutoRegisterIn<IGameplayScope>(Lifetime = NhemLifetime.Scoped)]
[EntryPoint]
public sealed class GameplayBootstrap : IStartable
{
    public void Start() { }
}
```

Generated output:

```csharp
builder.RegisterEntryPoint<GameplayBootstrap>();
```

Scene component:

```csharp
[AutoRegisterIn<IGameplayScope>(Lifetime = NhemLifetime.Scoped)]
[RegisterComponentInHierarchy]
[As<IPlayerView>]
public sealed class PlayerView : MonoBehaviour, IPlayerView
{
}
```

Generated output:

```csharp
builder.RegisterComponentInHierarchy<PlayerView>()
    .As<IPlayerView>();
```

## Analyzer MVP

- `NHEM_DI_001`: invalid `As<TContract>`
- `NHEM_DI_002`: invalid scope marker
- `NHEM_DI_003`: missing `As<TContract>` / `AsSelf`
- `NHEM_DI_010`: missing `LifetimeScopeFor<TScope>` mapping
- `NHEM_DI_011`: mapped scope does not call `RegisterGeneratedFor<TScope>()`
- `NHEM_DI_012`: mapped scope calls the wrong marker
- `NHEM_DI_022`: generated registration invoked more than once
- `NHEM_DI_040`: invalid `[EntryPoint]`
- `NHEM_DI_050`: `IObjectResolver` injected into regular service

## Non-goals in the current rollout

- No generated `LifetimeScope` MonoBehaviours.
- No partial injection into user scopes.
- No generated resolver callback paths.
- No runtime resolver caches or service locator state.
- No forced folder layout or forced architecture.

## Build and test

```bash
dotnet test Source~/NhemDangFugBixs.Tooling.sln -c Release
```

Release gate:

```powershell
./scripts/release-gate.ps1
```

Optional Unity smoke compile:

```powershell
$env:UNITY_EXE = "C:\Program Files\Unity\Hub\Editor\<version>\Editor\Unity.exe"
$env:NHEM_UNITY_PROJECT_ROOT = "I:\unityVers\Unity-Sameple\VContainerSourceGenSample"
./scripts/release-gate.ps1
```

If `UNITY_EXE` is not set, the script reports Unity sample compile as `SKIPPED`.
When Unity is configured, the compile log is written to `artifacts/unity-sample-compile.log`.

## Docs

Astro Starlight docs live in:

- `Source~/nhemdangfugbixs-tooling-docs`
