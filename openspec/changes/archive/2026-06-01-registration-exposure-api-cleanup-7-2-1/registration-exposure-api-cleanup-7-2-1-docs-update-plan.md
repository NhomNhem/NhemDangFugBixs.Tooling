# Docs Update Plan: Registration Exposure API Cleanup 7.2.1

## Overview

This document outlines the documentation updates required for version 7.2.1 to reflect the canonical explicit exposure API changes.

## Files to Update

### 1. README.md

**Location**: Root of repository

**Changes Required**:

#### Section: Minimal Usage Example

**Current** (if legacy-first):
```markdown
Service asmdef:

```csharp
using NhemDangFugBixs.Attributes;

public interface ICombatCoreService { }

[AutoRegisterIn<IGameplayScope>(Lifetime = NhemLifetime.Scoped, AsImplementedInterfaces = true)]
public sealed class CombatCoreService : ICombatCoreService
{
}
```
```

**Updated** (canonical-first):
```markdown
Service asmdef (canonical explicit style):

```csharp
using NhemDangFugBixs.Attributes;

public interface ICombatCoreService { }

[AutoRegisterIn<IGameplayScope>(Lifetime = NhemLifetime.Scoped)]
[As<ICombatCoreService>]
public sealed class CombatCoreService : ICombatCoreService
{
}
```

Legacy flag-style (still supported):

```csharp
[AutoRegisterIn<IGameplayScope>(Lifetime = NhemLifetime.Scoped, AsImplementedInterfaces = true)]
public sealed class CombatCoreService : ICombatCoreService
{
}
```
```

#### Section: Phase 2 Examples

**Current**:
```markdown
Phase 2 examples:

```csharp
[AutoRegisterIn<IGameplayScope>(Lifetime = NhemLifetime.Scoped)]
[EntryPoint]
public sealed class GameplayLoopEntryPoint : IStartable
{
    public void Start() { }
}
```

```csharp
[AutoRegisterIn<IGameplayScope>(Lifetime = NhemLifetime.Scoped)]
[RegisterComponentInHierarchy]
[As<IPlayerView>]
public sealed class PlayerView : MonoBehaviour, IPlayerView
{
}
```
```

**Updated** (ensure consistency, these already look good but verify):
```markdown
Phase 2 examples:

```csharp
[AutoRegisterIn<IGameplayScope>(Lifetime = NhemLifetime.Scoped)]
[EntryPoint]
public sealed class GameplayLoopEntryPoint : IStartable
{
    public void Start() { }
}
```

```csharp
[RegisterComponentInHierarchy]
[AutoRegisterIn<IGameplayScope>(Lifetime = NhemLifetime.Scoped)]
[As<IPlayerView>]
public sealed class PlayerView : MonoBehaviour, IPlayerView
{
}
```
```

#### New Section: Migration Guide for 7.2.1

**Add after "Phase 2 examples" section**:

```markdown
## Migration Guide for 7.2.1

### Registration Exposure API

Version 7.2.1 introduces canonical explicit exposure attributes.

**Before (legacy flag-style):**
```csharp
[AutoRegisterIn<IGameplayScope>(AsImplementedInterfaces = true, AsSelf = false)]
public sealed class Service : IService { }
```

**After (canonical explicit style):**
```csharp
[AutoRegisterIn<IGameplayScope>]
[As<IService>]
public sealed class Service : IService { }
```

**Migration notes:**
- Legacy flag-style remains fully supported for backward compatibility
- New diagnostic NHEM_DI_060 warns when mixing explicit attributes with legacy flags
- Explicit attributes provide clearer intent and better IDE support
- No breaking changes to existing code

**See the full migration guide for detailed scenarios.**
```

#### Section: Public API (Phase 1/2)

**Current** (already correct, verify):
```markdown
## Public API (Phase 1/2)

- `AutoRegisterIn<TScope>` / `AutoRegisterIn(typeof(TScope))`
- `As<TContract>` / `As(typeof(TContract))`
- `AsSelf`
- `LifetimeScopeFor<TScope>` / `LifetimeScopeFor(typeof(TScope))`
- `EntryPoint`
- `RegisterComponentInHierarchy`
- `builder.RegisterGeneratedFor<TScope>()`
```

**No changes needed** - this section already lists the canonical API correctly.

### 2. AutoRegisterInAttribute.cs XML Documentation

**Location**: `Source~/DangFugBixs.Attributes~/Attributes/AutoRegisterInAttribute.cs`

**Changes Required**:

#### Update Summary Comment

**Current**:
```csharp
/// <summary>
/// Registers a class with VContainer using type-safe scope reference.
/// Supports multi-contract registration with explicit control over interface bindings.
/// 
/// The scope type should inherit from VContainer.Unity.LifetimeScope, OR it can be an
/// Identity Type mapped via [LifetimeScopeFor] for cross-assembly discovery.
///
/// Note: Uses NhemDangFugBixs.Attributes.NhemLifetime which has the same values as VContainer.Lifetime.
/// In your Unity project files, use the fully qualified name or an alias to avoid ambiguity.
/// </summary>
```

**Updated**:
```csharp
/// <summary>
/// Registers a class with VContainer using type-safe scope reference.
/// 
/// Canonical usage: Use [As] and [AsSelf] attributes for contract exposure.
/// AutoRegisterIn should only declare scope and lifetime.
/// 
/// The scope type should inherit from VContainer.Unity.LifetimeScope, OR it can be an
/// Identity Type mapped via [LifetimeScopeFor] for cross-assembly discovery.
///
/// Note: Uses NhemDangFugBixs.Attributes.NhemLifetime which has the same values as VContainer.Lifetime.
/// In your Unity project files, use the fully qualified name or an alias to avoid ambiguity.
/// </summary>
```

#### Update XML Examples

**Current** (example 2 shows mixed style):
```csharp
/// <example>
/// <code>
/// // Basic Usage - Auto-detect all interfaces (default)
/// [AutoRegisterIn(typeof(GameScope))]
/// public class GameService : IGameService, ITickable { }
/// // Generates: builder.RegisterEntryPoint&lt;GameService&gt;().AsImplementedInterfaces().AsSelf();
///
/// // Explicit Contracts - Specify exactly which interfaces to register
/// [AutoRegisterIn(typeof(GameScope), AsTypes = [typeof(IGameService)])]
/// public class GameService : IGameService, ITickable { }
/// // Generates: builder.RegisterEntryPoint&lt;GameService&gt;().As&lt;IGameService&gt;();
///
/// // Disable Auto-Detection - Only register as self
/// [AutoRegisterIn(typeof(GameScope), AsImplementedInterfaces = false)]
/// public class GameService : IGameService, ITickable { }
/// // Generates: builder.RegisterEntryPoint&lt;GameService&gt;().AsSelf();
///
/// // Multiple Explicit Contracts
/// [AutoRegisterIn(typeof(GameScope), AsTypes = [typeof(IGameService), typeof(ITickable)])]
/// public class GameService : IGameService, ITickable { }
/// // Generates: builder.RegisterEntryPoint&lt;GameService&gt;().As&lt;IGameService&gt;().As&lt;ITickable&gt;();
///
/// // Identity Type (Cross-Layer / Decoupled)
/// [AutoRegisterIn(typeof(GameScope))]  // GameScope is empty type in shared assembly
/// public class GameService : IGameService { }
/// </code>
/// </example>
```

**Updated** (canonical-first, remove mixed style examples):
```csharp
/// <example>
/// <code>
/// // Canonical - Explicit contract exposure
/// [AutoRegisterIn(typeof(GameScope), Lifetime = NhemLifetime.Scoped)]
/// [As(typeof(IGameService))]
/// public class GameService : IGameService, ITickable { }
/// // Generates: builder.Register&lt;GameService&gt;(Lifetime.Scoped).As&lt;IGameService&gt;();
///
/// // Canonical - Multiple explicit contracts
/// [AutoRegisterIn(typeof(GameScope), Lifetime = NhemLifetime.Scoped)]
/// [As(typeof(IGameService))]
/// [As(typeof(ITickable))]
/// public class GameService : IGameService, ITickable { }
/// // Generates: builder.Register&lt;GameService&gt;(Lifetime.Scoped).As&lt;IGameService&gt;().As&lt;ITickable&gt;();
///
/// // Canonical - Self-only registration
/// [AutoRegisterIn(typeof(GameScope), Lifetime = NhemLifetime.Scoped)]
/// [AsSelf]
/// public class GameService { }
/// // Generates: builder.Register&lt;GameService&gt;(Lifetime.Scoped).AsSelf();
///
/// // Legacy - Flag-based exposure (still supported for compatibility)
/// [AutoRegisterIn(typeof(GameScope), AsImplementedInterfaces = true)]
/// public class GameService : IGameService, ITickable { }
/// // Generates: builder.Register&lt;GameService&gt;().AsImplementedInterfaces().AsSelf();
///
/// // Legacy - Disable auto-detection, only register as self
/// [AutoRegisterIn(typeof(GameScope), AsImplementedInterfaces = false)]
/// public class GameService : IGameService, ITickable { }
/// // Generates: builder.Register&lt;GameService&gt;().AsSelf();
///
/// // Identity Type (Cross-Layer / Decoupled)
/// [AutoRegisterIn(typeof(GameScope))]  // GameScope is empty type in shared assembly
/// public class GameService : IGameService { }
/// </code>
/// </example>
```

#### Update Property Documentation

**Current** (AsImplementedInterfaces property):
```csharp
/// <summary>
/// Whether to bind to all implemented interfaces.
/// Default: true (auto-detect all interfaces except VContainer lifecycle interfaces).
/// Set to false to disable auto-detection and use explicit AsTypes only.
/// </summary>
```

**Updated**:
```csharp
/// <summary>
/// Whether to bind to all implemented interfaces.
/// Default: true (auto-detect all interfaces except VContainer lifecycle interfaces).
/// Set to false to disable auto-detection and use explicit AsTypes only.
/// 
/// Note: For new code, prefer using explicit [As] attributes instead of this flag.
/// This flag is maintained for backward compatibility.
/// </summary>
```

**Current** (AsSelf property):
```csharp
/// <summary>
/// Whether to bind to self (the concrete type).
/// Default: true. Set to false if you only want interface registrations.
/// </summary>
```

**Updated**:
```csharp
/// <summary>
/// Whether to bind to self (the concrete type).
/// Default: true. Set to false if you only want interface registrations.
/// 
/// Note: For new code, prefer using explicit [AsSelf] attribute instead of this flag.
/// This flag is maintained for backward compatibility.
/// </summary>
```

**Current** (AsTypes property):
```csharp
/// <summary>
/// Explicit interface types to bind to.
/// When specified, this OVERRIDES AsImplementedInterfaces (only these contracts will be registered).
/// Use this for precise control over which interfaces are exposed.
/// 
/// Example: AsTypes = [typeof(IService1), typeof(IService2)]
/// </summary>
```

**Updated**:
```csharp
/// <summary>
/// Explicit interface types to bind to.
/// When specified, this takes precedence over AsImplementedInterfaces flag.
/// Use this for precise control over which interfaces are exposed.
/// 
/// Note: For new code, prefer using [As] attributes instead of this property.
/// This property is maintained for backward compatibility.
/// 
/// Example: AsTypes = [typeof(IService1), typeof(IService2)]
/// </summary>
```

### 3. CHANGELOG.md

**Location**: Root of repository

**Changes Required**:

Add new section after 7.2.0:

```markdown
## 7.2.1 - 2026-05-17

### Added
- New diagnostic NHEM_DI_060 to warn when mixing explicit [As]/[AsSelf] attributes with legacy AutoRegisterIn exposure flags
- Deterministic contract ordering in generated registration output (alphabetically sorted)
- Migration guide for adopting canonical explicit exposure attributes

### Changed
- Explicit [As] and [AsSelf] attributes now take precedence over legacy AsImplementedInterfaces and AsSelf flags
- Documentation updated to emphasize canonical explicit exposure style as recommended approach
- AutoRegisterInAttribute XML docs updated with canonical-first examples
- README updated with migration guide for 7.2.1

### Deprecated
- Legacy flag-style exposure (AsImplementedInterfaces, AsSelf flags) is not deprecated but canonical explicit style is recommended
- NHEM_DI_060 warning encourages migration to explicit style but does not force it

### Compatibility
- Legacy flag-style remains fully supported for backward compatibility
- No breaking changes to existing code
- No changes to generated output for legacy-style code
```

### 4. Samples (if applicable)

**Location**: `Samples~/` directory

**Changes Required**:

#### Review all sample files for exposure style

**Files to check**:
- Any files with `[AutoRegisterIn]` attributes
- Any files demonstrating registration patterns

**Action**:
1. Update new samples to use canonical explicit style
2. Keep a few legacy-style samples for reference (marked as "legacy")
3. Ensure EntryPoint examples implementing VContainer.Unity interfaces live in Composition assemblies
4. Ensure service-only asmdef samples do not reference VContainer or VContainer.Unity

**Example conversion**:

**Before**:
```csharp
[AutoRegisterIn<IGameplayScope>(AsImplementedInterfaces = true)]
public sealed class CombatService : ICombatService
{
}
```

**After**:
```csharp
[AutoRegisterIn<IGameplayScope>]
[As<ICombatService>]
public sealed class CombatService : ICombatService
{
}
```

## Documentation Review Checklist

### README.md
- [ ] Minimal Usage Example updated to canonical-first
- [ ] Phase 2 examples verified for consistency
- [ ] Migration Guide section added
- [ ] Public API section verified (no changes needed)
- [ ] All examples compile and are accurate

### AutoRegisterInAttribute.cs
- [ ] Summary comment updated to mention canonical usage
- [ ] XML examples updated to canonical-first
- [ ] Mixed style examples removed
- [ ] Property documentation updated with backward compatibility notes
- [ ] All examples compile and are accurate

### CHANGELOG.md
- [ ] 7.2.1 section added
- [ ] All changes documented
- [ ] Compatibility notes included
- [ ] Version date correct

### Samples
- [ ] New samples use canonical explicit style
- [ ] Legacy samples marked as such
- [ ] EntryPoint placement verified
- [ ] Service-only asmdef independence verified

## Documentation Testing

### Compile Examples
1. Extract all code examples from documentation
2. Verify they compile with the package
3. Verify generated output matches documentation

### Link Validation
1. Verify all internal links work
2. Verify all external links are current
3. Check for broken references

### Consistency Check
1. Ensure all examples use consistent style
2. Ensure terminology is consistent across files
3. Ensure version numbers are consistent

## Documentation Release Notes

### What Users Need to Know

1. **New diagnostic**: NHEM_DI_060 warns about mixed exposure style
2. **Canonical style**: Explicit [As]/[AsSelf] attributes are now recommended
3. **Backward compatibility**: Legacy flag-style remains supported
4. **No breaking changes**: Existing code continues to work
5. **Migration guide**: Available for those who want to adopt canonical style

### Communication Points

1. Emphasize that this is a **recommendation**, not a requirement
2. Highlight that the diagnostic is a **warning**, not an error
3. Provide clear migration path for those who want to adopt canonical style
4. Reassure that existing code is safe and will continue to work

## Documentation Rollout Plan

### Phase 1: Update Draft Documentation
- Update all documentation files
- Review internally
- Verify examples compile

### Phase 2: Update Public Documentation
- Commit documentation changes
- Update package with new documentation
- Verify documentation in package

### Phase 3: Announce
- Update release notes
- Highlight changes in README
- Point users to migration guide

## Documentation Maintenance

### Future Considerations

1. **Deprecation timeline**: Consider deprecating legacy flags in a future major version
2. **Code fix provider**: Consider adding code fix for NHEM_DI_060 in future
3. **Sample updates**: Continue updating samples as patterns evolve
4. **Documentation reviews**: Regular reviews to keep documentation current
