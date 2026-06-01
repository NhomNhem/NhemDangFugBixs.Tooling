# Analyzer Spec Delta: Analyzer Maturity (7.3.0)

## New Diagnostics

### NHEM_DI_061 — Duplicate explicit contract exposure

**ID:** NHEM_DI_061
**Title:** Duplicate explicit contract exposure
**Severity:** Warning
**Category:** NhemDangFugBixs
**Enabled by default:** Yes

**Description:**
A type declares duplicate `[As(...)]` attributes for the same contract type. This is redundant and may indicate a copy-paste error or confusion about explicit contract exposure.

**Message:**
```
Duplicate contract exposure. Remove duplicate [As] declaration for the same contract.
```

**Example (bad):**
```csharp
[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemLifetime.Scoped)]
[As(typeof(IPlayerView))]
[As(typeof(IPlayerView))]
public sealed class PlayerView : MonoBehaviour, IPlayerView
{
}
```

**Example (good):**
```csharp
[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemLifetime.Scoped)]
[As(typeof(IPlayerView))]
public sealed class PlayerView : MonoBehaviour, IPlayerView
{
}
```

**Example (good - multiple different contracts):**
```csharp
[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemLifetime.Scoped)]
[As(typeof(IPlayerView))]
[As(typeof(ICombatTarget))]
public sealed class PlayerView : MonoBehaviour, IPlayerView, ICombatTarget
{
}
```

### NHEM_DI_066 — RegisterComponentInHierarchy on non-MonoBehaviour

**ID:** NHEM_DI_066
**Title:** RegisterComponentInHierarchy on non-MonoBehaviour
**Severity:** Error
**Category:** NhemDangFugBixs
**Enabled by default:** Yes

**Description:**
A type uses `[RegisterComponentInHierarchy]` but does not inherit from `UnityEngine.MonoBehaviour`. This attribute is only valid for MonoBehaviour types that can be instantiated in the Unity scene hierarchy.

**Message:**
```
RegisterComponentInHierarchy can only be used on MonoBehaviour types.
```

**Example (bad):**
```csharp
[RegisterComponentInHierarchy]
[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemLifetime.Scoped)]
public sealed class PlayerViewService
{
}
```

**Example (good):**
```csharp
[RegisterComponentInHierarchy]
[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemLifetime.Scoped)]
public sealed class PlayerView : MonoBehaviour
{
}
```

### NHEM_DI_067 — EntryPoint without known lifecycle contract

**ID:** NHEM_DI_067
**Title:** EntryPoint without known lifecycle contract
**Severity:** Warning
**Category:** NhemDangFugBixs
**Enabled by default:** Yes

**Description:**
A type uses `[EntryPoint]` but does not implement a known VContainer lifecycle interface. EntryPoint is used to register lifecycle callbacks, but without a lifecycle interface, the registration may not behave as expected.

**Message:**
```
EntryPoint should implement a known lifecycle interface such as IStartable, ITickable, IInitializable, or IDisposable.
```

**Known lifecycle interfaces:**
- VContainer.Unity.IInitializable
- VContainer.Unity.IStartable
- VContainer.Unity.IPostInitializable
- VContainer.Unity.ITickable
- VContainer.Unity.IFixedTickable
- VContainer.Unity.ILateTickable
- System.IDisposable

**Example (bad):**
```csharp
[EntryPoint]
[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemLifetime.Scoped)]
public sealed class GameplayLoopEntryPoint
{
}
```

**Example (good):**
```csharp
[EntryPoint]
[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemLifetime.Scoped)]
public sealed class GameplayLoopEntryPoint : IStartable
{
    public void Start() { }
}
```

**Example (good):**
```csharp
[EntryPoint]
[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemLifetime.Scoped)]
public sealed class GameplayLoopEntryPoint : ITickable
{
    public void Tick() { }
}
```

## Implementation Notes

- All diagnostics use semantic type comparison via SymbolEqualityComparer.Default
- Missing UnityEngine or VContainer.Unity references are handled gracefully
- Diagnostics only apply to types with [AutoRegisterIn] attributes
- No assembly scanning beyond the current compilation
