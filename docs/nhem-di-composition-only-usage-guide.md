# Nhem DI Composition-Only Usage Guide

> **Note on attribute names:** This guide documents the **current implemented API**.
> The attributes in `NhemDangFugBixs.Attributes` do **not** carry a `Nhem` prefix.
> If your project requires prefixed names, add `using` aliases or request a future API rename.
>
> Current names: `[AutoRegisterIn]`, `[As]`, `[LifetimeScopeFor]`, `[EntryPoint]`, etc.

> **Naming Note:** The current public API intentionally uses concise attribute names such as `[AutoRegisterIn]`, `[As]`, `[LifetimeScopeFor]`, and `[EntryPoint]`. Earlier design notes may mention `Nhem*` prefixes, but those are **not implemented** in this release candidate. Renaming to `Nhem*` would be a breaking API change and is deferred to a future API-polish proposal.

---

## 1. Mental Model

Nhem DI is a compile-time source generator for VContainer:

1. You mark classes with attributes.
2. The source generator emits VContainer registration code at compile time.
3. In **composition-only mode**, only assemblies containing `[LifetimeScopeFor<TScope>]` emit generated installers.
4. Service-only assemblies declare registration intent but do **not** emit installers.
5. `di-smoke` validates the multi-assembly setup after build.

You never write `builder.Register<T>()` by hand for attributed services.

---

## 2. Define Scope Markers

A scope marker is an empty interface that acts as a "name" for a scope.
It lives in a shared assembly so other assemblies can reference it without depending on VContainer.

```csharp
// Game.Shared.dll
public interface IGameplayScope : INhemScopeMarker { }
public interface IMainMenuScope : INhemScopeMarker { }
```

`INhemScopeMarker` is defined in `NhemDangFugBixs.Attributes`.

---

## 3. Mark the Composition Target

The composition target is a VContainer `LifetimeScope` that maps a marker to an actual VContainer scope.

```csharp
// Game.Composition.dll
using VContainer;
using VContainer.Unity;
using NhemDangFugBixs.Attributes;

[LifetimeScopeFor<IGameplayScope>]
public sealed class GameplayLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterGeneratedFor<IGameplayScope>();
    }
}
```

Or with `typeof(...)` compatibility syntax:

```csharp
[LifetimeScopeFor(typeof(IGameplayScope))]
public sealed class GameplayLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterGeneratedFor<IGameplayScope>();
    }
}
```

**Important:** `RegisterGeneratedFor<TScope>()` is an extension method emitted by the generator into `NhemGeneratedVContainerExtensions`.
It routes to the correct generated installer for that scope marker.

---

## 4. Register Services

Mark a service with the scope it belongs to and the contracts it exposes.

### Preferred syntax (generic attributes, C# 11+)

```csharp
// Game.Application.dll
using NhemDangFugBixs.Attributes;

[AutoRegisterIn<IGameplayScope>(Lifetime = NhemLifetime.Scoped)]
[As<ICombatCore>]
public sealed class CombatCore : ICombatCore
{
}
```

### Compatibility syntax (`typeof(...)`)

```csharp
[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemLifetime.Scoped)]
[As(typeof(ICombatCore))]
public sealed class CombatCore : ICombatCore
{
}
```

### Auto-detect interfaces (default behavior)

```csharp
[AutoRegisterIn<IGameplayScope>(Lifetime = NhemLifetime.Scoped)]
public sealed class CombatCore : ICombatCore, IInitializable
{
}
```

Generated:

```csharp
builder.Register<global::Game.Application.CombatCore>(global::VContainer.Lifetime.Scoped)
       .AsImplementedInterfaces()  // ICombatCore, IInitializable
       .AsSelf();
```

### Explicit contracts only

```csharp
[AutoRegisterIn<IGameplayScope>(Lifetime = NhemLifetime.Scoped)]
[As<ICombatCore>]
public sealed class CombatCore : ICombatCore, IInitializable
{
}
```

Generated:

```csharp
builder.Register<global::Game.Application.CombatCore>(global::VContainer.Lifetime.Scoped)
       .As<global::Game.Application.ICombatCore>()
       .AsSelf();
```

### Self-only (no interface binding)

```csharp
[AutoRegisterIn<IGameplayScope>(Lifetime = NhemLifetime.Scoped, AsImplementedInterfaces = false)]
public sealed class CombatCore : ICombatCore
{
}
```

Generated:

```csharp
builder.Register<global::Game.Application.CombatCore>(global::VContainer.Lifetime.Scoped)
       .AsSelf();
```

---

## 5. Register Entry Points

Entry points are VContainer lifecycle types (`ITickable`, `IInitializable`, `IPostInitializable`, etc.).
They are registered separately from normal services.

```csharp
[AutoRegisterIn<IGameplayScope>]
[EntryPoint]
public sealed class GameplayTickHandler : ITickable
{
    public void Tick()
    {
        // ...
    }
}
```

Generated:

```csharp
builder.RegisterEntryPoint<global::Game.Application.GameplayTickHandler>();
```

**Do not** rely on `.AsImplementedInterfaces()` for `ITickable`, `IInitializable`, or other VContainer entry point interfaces. Use `[EntryPoint]` explicitly.

---

## 6. Register Unity Components

### Scene components (already in hierarchy)

```csharp
[SceneComponent<IGameplayScope>]
public sealed class DebugOverlayAdapter : MonoBehaviour
{
}
```

Or with `typeof(...)`:

```csharp
[SceneComponent(typeof(IGameplayScope))]
public sealed class DebugOverlayAdapter : MonoBehaviour
{
}
```

Generated:

```csharp
builder.RegisterComponentInHierarchy<global::Game.Application.DebugOverlayAdapter>();
```

### New GameObject components

```csharp
[NewGameObjectComponent<IGameplayScope>(
    Lifetime = NhemLifetime.Singleton,
    Name = "AudioManager")]
public sealed class AudioManager : MonoBehaviour
{
}
```

Generated:

```csharp
builder.RegisterComponentOnNewGameObject<global::Game.Application.AudioManager>(
    global::VContainer.Lifetime.Singleton,
    "AudioManager");
```

Without a name:

```csharp
[NewGameObjectComponent<IGameplayScope>(Lifetime = NhemLifetime.Singleton)]
public sealed class AudioManager : MonoBehaviour
{
}
```

Generated:

```csharp
builder.RegisterComponentOnNewGameObject<global::Game.Application.AudioManager>(
    global::VContainer.Lifetime.Singleton);
```

---

## 7. Multi-Assembly Composition-Only Flow

```text
Game.Shared.dll
- INhemScopeMarker
- IGameplayScope

Game.Application.dll
- [AutoRegisterIn<IGameplayScope>] CombatCore
- No generated installer emitted here

Game.Composition.dll
- [LifetimeScopeFor<IGameplayScope>] GameplayLifetimeScope
- Calls builder.RegisterGeneratedFor<IGameplayScope>()
- Generated installer emitted here, containing:
  builder.Register<CombatCore>(Lifetime.Scoped).As<ICombatCore>().AsSelf();
```

**Rule:** Only the assembly containing `[LifetimeScopeFor]` emits the generated installer.
Service-only assemblies declare intent via attributes but do not emit code.

---

## 8. Generated Code Examples

For a scope `IGameplayScope` with services `CombatCore` and `AudioManager`, the generator emits:

```csharp
namespace NhemDangFugBixs.Generated.Game_Composition
{
    public static partial class GameplayScopeInstaller
    {
        public static void Register(global::VContainer.IContainerBuilder builder)
        {
            if (builder == null) throw new global::System.ArgumentNullException(nameof(builder));
            RegisterServices(builder);
            RegisterEntryPoints(builder);
            RegisterComponents(builder);
        }

        private static void RegisterServices(global::VContainer.IContainerBuilder builder)
        {
            builder.Register<global::Game.Application.CombatCore>(global::VContainer.Lifetime.Scoped)
                   .As<global::Game.Application.ICombatCore>()
                   .AsSelf();
        }

        private static void RegisterEntryPoints(global::VContainer.IContainerBuilder builder)
        {
            // entry points here
        }

        private static void RegisterComponents(global::VContainer.IContainerBuilder builder)
        {
            // scene components / new game object components here
        }
    }
}
```

A compatibility facade is also generated:

```csharp
namespace NhemDangFugBixs.Generated.Game_Composition
{
    public static partial class VContainerRegistration
    {
        public static void RegisterAll(global::VContainer.IContainerBuilder builder)
        {
            GameplayScopeInstaller.Register(builder);
        }

        public static void RegisterGeneratedFor<TScopeMarker>(global::VContainer.IContainerBuilder builder)
        {
            global::NhemDangFugBixs.VContainer.NhemGeneratedVContainerExtensions
                .RegisterGeneratedFor<TScopeMarker>(builder);
        }
    }
}
```

And the extension method:

```csharp
namespace NhemDangFugBixs.VContainer
{
    public static partial class NhemGeneratedVContainerExtensions
    {
        public static void RegisterGeneratedFor<TScope>(this global::VContainer.IContainerBuilder builder)
        {
            if (builder == null) throw new global::System.ArgumentNullException(nameof(builder));
            var marker = typeof(TScope);
            if (marker == typeof(global::Game.Shared.IGameplayScope))
            {
                global::NhemDangFugBixs.Generated.Game_Composition.GameplayScopeInstaller.Register(builder);
                return;
            }
            throw new global::System.InvalidOperationException(
                $"No generated VContainer installer found for scope marker {marker.FullName}.");
        }
    }
}
```

---

## 9. Validation with di-smoke

### Single assembly

```bash
dotnet di-smoke validate Game.Composition.dll
```

### Multi-assembly (cross-asmdef)

```bash
dotnet di-smoke validate \
  Game.Shared.dll \
  Game.Application.dll \
  Game.Composition.dll
```

### Release gate

```powershell
pwsh ./scripts/release-gate.ps1
```

The release gate runs cross-asmdef validation after Unity compilation and fails if duplicate composition targets, orphan services, or missing VContainer references are detected.

---

## 10. Compatibility Syntax: `typeof(...)`

Generic attribute syntax requires C# 11 or later. For older language versions, use `typeof(...)`:

```csharp
// Generic (preferred)
[AutoRegisterIn<IGameplayScope>(Lifetime = NhemLifetime.Scoped)]
[As<ICombatCore>]

// typeof(...) fallback
[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemLifetime.Scoped)]
[As(typeof(ICombatCore))]
```

Both forms are treated identically by the generator.

---

## 11. Optional / Future Convenience Aliases

> **Warning:** These aliases are optional/future API. They are not part of core Phase 1 docs unless already implemented and tested.

```csharp
[GameplayService]      // Equivalent to [AutoRegisterIn<IGameplayScope>(Lifetime = Scoped)]
[MainMenuService]      // Equivalent to [AutoRegisterIn<IMainMenuScope>(Lifetime = Scoped)]
[ProjectService]       // Equivalent to [AutoRegisterIn<IProjectScope>(Lifetime = Singleton)]
```

If you need these, verify they exist in your package version before using them.

---

## 12. Common Mistakes

### Mistake: Broken generic syntax

Wrong:
```csharp
[NhemAutoRegisterIn<<IGameplayScope>]   // double <
```

Correct:
```csharp
[AutoRegisterIn<IGameplayScope>]
```

### Mistake: Registering entry points as normal services

Wrong:
```csharp
[AutoRegisterIn<IGameplayScope>(Lifetime = NhemLifetime.Scoped)]
public sealed class GameplayTickHandler : ITickable
```

Correct:
```csharp
[AutoRegisterIn<IGameplayScope>]
[EntryPoint]
public sealed class GameplayTickHandler : ITickable
```

### Mistake: Mapping NewGameObject to prefab registration

Wrong expectation:
```csharp
// This does NOT generate RegisterComponentInNewPrefab
[NewGameObjectComponent<IGameplayScope>]
```

Correct:
```csharp
[NewGameObjectComponent<IGameplayScope>(Name = "AudioManager")]
```

Generates `RegisterComponentOnNewGameObject<T>(lifetime, "name")`.
Prefab registration is a separate/future feature.

### Mistake: Composition assembly missing installer call

`[LifetimeScopeFor<TScope>]` marks the composition target, but the `Configure` method must call:

```csharp
builder.RegisterGeneratedFor<TScope>();
```

Without this call, the generated installer is never invoked at runtime.

### Mistake: Service-only assembly expecting generated code

Assemblies without `[LifetimeScopeFor]` do not emit installers.
They only declare service intent via `[AutoRegisterIn]`.
The composition assembly is responsible for emitting the combined installer.
