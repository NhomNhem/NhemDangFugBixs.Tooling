# Migration Guide: Registration Exposure API Cleanup 7.2.1

## Overview

Version 7.2.1 introduces canonical explicit exposure attributes for VContainer registration. This guide helps you migrate from legacy flag-style registration to the recommended explicit attribute style.

## What's New

### New Diagnostic: NHEM_DI_060

A new analyzer diagnostic warns when you mix explicit exposure attributes `[As]` or `[AsSelf]` with legacy `AutoRegisterIn` exposure flags (`AsImplementedInterfaces` or `AsSelf`).

**Severity**: Warning  
**Message**: "Mixed registration exposure style. Prefer explicit [As] / [AsSelf]; AutoRegisterIn should only declare scope and lifetime."

### Canonical API Separation

The registration API now has clear separation of concerns:

- **AutoRegisterIn**: Declares scope and lifetime only
- **[As]** / **[AsSelf]**: Declare contract exposure
- **[EntryPoint]**: Declares VContainer lifecycle registration
- **[RegisterComponentInHierarchy]**: Declares Unity component registration kind

## Migration Scenarios

### Scenario 1: Basic Service with Single Interface

**Before (legacy flag-style):**
```csharp
[AutoRegisterIn<IGameplayScope>(AsImplementedInterfaces = true, AsSelf = true)]
public sealed class CombatService : ICombatService
{
    // Implementation
}
```

**After (canonical explicit style):**
```csharp
[AutoRegisterIn<IGameplayScope>]
[As<ICombatService>]
[AsSelf]
public sealed class CombatService : ICombatService
{
    // Implementation
}
```

**Or if you only need interface registration:**
```csharp
[AutoRegisterIn<IGameplayScope>]
[As<ICombatService>]
public sealed class CombatService : ICombatService
{
    // Implementation
}
```

### Scenario 2: Service with Multiple Interfaces

**Before (legacy flag-style):**
```csharp
[AutoRegisterIn<IGameplayScope>(AsImplementedInterfaces = true)]
public sealed class DataService : IDataService, IInitializable
{
    // Implementation
}
```

**After (canonical explicit style):**
```csharp
[AutoRegisterIn<IGameplayScope>]
[As<IDataService>]
[As<IInitializable>]
public sealed class DataService : IDataService, IInitializable
{
    // Implementation
}
```

### Scenario 3: Component in Hierarchy

**Before (mixed style - triggers NHEM_DI_060):**
```csharp
[RegisterComponentInHierarchy]
[AutoRegisterIn<IGameplayScope>(AsImplementedInterfaces = false, AsSelf = false)]
[As<IPlayerView>]
public sealed class PlayerView : MonoBehaviour, IPlayerView
{
    // Implementation
}
```

**After (canonical explicit style):**
```csharp
[RegisterComponentInHierarchy]
[AutoRegisterIn<IGameplayScope>]
[As<IPlayerView>]
public sealed class PlayerView : MonoBehaviour, IPlayerView
{
    // Implementation
}
```

### Scenario 4: Entry Point with Lifecycle

**Before (legacy flag-style):**
```csharp
[AutoRegisterIn<IGameplayScope>(AsImplementedInterfaces = true)]
[EntryPoint]
public sealed class GameplayLoop : IStartable
{
    public void Start() { }
}
```

**After (canonical explicit style):**
```csharp
[AutoRegisterIn<IGameplayScope>]
[EntryPoint]
public sealed class GameplayLoop : IStartable
{
    public void Start() { }
}
```

**Note**: Entry points typically don't need explicit `[As]` attributes because VContainer handles lifecycle interfaces automatically.

### Scenario 5: Self-Only Registration

**Before (legacy flag-style):**
```csharp
[AutoRegisterIn<IGameplayScope>(AsImplementedInterfaces = false, AsSelf = true)]
public sealed class InternalService
{
    // Implementation
}
```

**After (canonical explicit style):**
```csharp
[AutoRegisterIn<IGameplayScope>]
[AsSelf]
public sealed class InternalService
{
    // Implementation
}
```

## Step-by-Step Migration

### Step 1: Identify Mixed Usage

Run your project and look for NHEM_DI_060 warnings. These indicate where you're mixing explicit attributes with legacy flags.

### Step 2: Choose Your Approach

You have two options:

**Option A: Migrate to Explicit Style (Recommended)**
- Remove legacy flags from `AutoRegisterIn`
- Add explicit `[As]` and `[AsSelf]` attributes
- Clearer intent, better IDE support

**Option B: Keep Legacy Style**
- Remove explicit `[As]` and `[AsSelf]` attributes
- Keep using legacy flags
- No code changes needed for existing behavior

### Step 3: Apply Changes

For each NHEM_DI_060 warning:

1. Decide which contracts you want to expose
2. Add `[As<TContract>]` for each interface
3. Add `[AsSelf]` if you want self-registration
4. Remove `AsImplementedInterfaces` and `AsSelf` flags from `AutoRegisterIn`

### Step 4: Verify Generated Output

Check the generated registration code to ensure it matches your intent:

```csharp
// Expected for explicit style:
builder.Register<CombatService>(Lifetime.Scoped)
    .As<ICombatService>()
    .AsSelf();
```

## Backward Compatibility

**Important**: Legacy flag-style remains fully supported. You are not required to migrate.

- Existing code continues to work exactly as before
- No breaking changes to generated output
- NHEM_DI_060 is a warning, not an error
- You can suppress the warning if you prefer legacy style

## Benefits of Explicit Style

1. **Clearer Intent**: Explicit attributes make contract exposure obvious at a glance
2. **Better IDE Support**: IDE can provide better autocomplete and navigation
3. **No Confusion**: No ambiguity about which mechanism takes precedence
4. **Consistent with Modern VContainer**: Aligns with manual VContainer registration patterns
5. **Future-Proof**: Explicit style is the recommended path forward

## Common Questions

### Q: Do I have to migrate?

**A**: No. Legacy flag-style remains fully supported. The new diagnostic is a warning to help you identify mixed usage, not a requirement to change.

### Q: Will my existing code break?

**A**: No. All existing code continues to work exactly as before. The changes are additive, not breaking.

### Q: What if I want to keep using flags?

**A**: That's fine. Just don't use explicit `[As]` or `[AsSelf]` attributes together with flags. The diagnostic only warns about mixed usage.

### Q: Which style should I use for new code?

**A**: Use explicit `[As]` and `[AsSelf]` attributes. This is the canonical style going forward.

### Q: Can I mix styles in the same project?

**A**: Technically yes, but we recommend consistency. Choose one style and use it throughout your project to avoid confusion.

## Quick Reference

### Attribute Responsibility

| Attribute | Responsibility |
|-----------|---------------|
| `AutoRegisterIn` | Scope and lifetime |
| `[As<T>]` | Interface contract exposure |
| `[AsSelf]` | Self-registration |
| `[EntryPoint]` | VContainer lifecycle registration |
| `[RegisterComponentInHierarchy]` | Unity hierarchy component registration |

### Canonical Pattern

```csharp
[AutoRegisterIn<IScope>(Lifetime = NhemLifetime.Scoped)]
[As<IContract1>]
[As<IContract2>]
[AsSelf]
public sealed class Service : IContract1, IContract2
{
}
```

### Legacy Pattern (Still Supported)

```csharp
[AutoRegisterIn<IScope>(Lifetime = NhemLifetime.Scoped, AsImplementedInterfaces = true, AsSelf = true)]
public sealed class Service : IContract1, IContract2
{
}
```

## Getting Help

If you encounter issues during migration:

1. Check the generated registration code to verify output matches intent
2. Review the diagnostic messages for guidance
3. Refer to the updated README.md for examples
4. Check the samples in the package for canonical patterns
